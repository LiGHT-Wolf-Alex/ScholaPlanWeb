using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Только администраторы могут генерировать расписание
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScheduleController> _logger;
        private readonly IMapper _mapper;

        public ScheduleController(IScheduleService scheduleService, IUnitOfWork unitOfWork,
            ILogger<ScheduleController> logger, IMapper mapper)
        {
            _scheduleService = scheduleService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Генерация и сохранение расписания для школы с учетом предпочтений учителей.
        /// </summary>
        /// <param name="request">Запрос на генерацию расписания.</param>
        /// <returns></returns>
        [HttpPost("generate")]
        [Authorize(Roles = "Admin")] // Только администраторы могут генерировать расписание
        public async Task<ActionResult<ApiResponse<GenerateScheduleResponse>>> GenerateSchedule(
            [FromBody] GenerateScheduleRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при генерации расписания.");
                return BadRequest(new ApiResponse<GenerateScheduleResponse>(false, "Некорректные данные."));
            }

            var school = await _unitOfWork.Schools.GetByIdAsync(request.SchoolId);
            if (school == null)
            {
                _logger.LogWarning($"Школа с ID {request.SchoolId} не найдена при генерации расписания.");
                return NotFound(new ApiResponse<GenerateScheduleResponse>(false, "Школа не найдена."));
            }

            // Преобразование TeacherPreferencesDto в TeacherPreferences с использованием AutoMapper
            var teacherPreferences = _mapper.Map<Dictionary<int, TeacherPreferences>>(request.TeacherPreferences);

            try
            {
                var schedules = await _scheduleService.GenerateScheduleAsync(school, teacherPreferences);
                await _unitOfWork.LessonSchedules.AddRangeAsync(schedules);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation(
                    $"Расписание для школы ID {request.SchoolId} успешно сгенерировано и сохранено.");
                return Ok(new ApiResponse<GenerateScheduleResponse>(true,
                    "Расписание успешно сгенерировано и сохранено.",
                    new GenerateScheduleResponse
                        { Success = true, Message = "Расписание успешно сгенерировано и сохранено." }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации расписания.");
                return StatusCode(500,
                    new ApiResponse<GenerateScheduleResponse>(false, "Ошибка сервера при генерации расписания."));
            }
        }


        /// <summary>
        /// Получение расписания по ID.
        /// </summary>
        /// <param name="id">ID расписания.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LessonSchedule>>> GetScheduleById(int id)
        {
            var schedule = await _unitOfWork.LessonSchedules.GetByIdAsync(id);
            if (schedule == null)
            {
                _logger.LogWarning($"Расписание с ID {id} не найдено.");
                return NotFound(new ApiResponse<LessonSchedule>(false, "Расписание не найдено."));
            }

            return Ok(new ApiResponse<LessonSchedule>(true, "Расписание найдено.", schedule));
        }

        /// <summary>
        /// Обновление существующего расписания.
        /// </summary>
        /// <param name="id">ID расписания.</param>
        /// <param name="lessonSchedule">Обновленные данные расписания.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<LessonSchedule>>> Update(int id,
            [FromBody] LessonSchedule lessonSchedule)
        {
            if (id != lessonSchedule.Id)
            {
                _logger.LogWarning(
                    $"ID расписания в URL ({id}) не совпадает с ID в теле запроса ({lessonSchedule.Id}).");
                return BadRequest(new ApiResponse<LessonSchedule>(false, "Несоответствие ID."));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при обновлении расписания.");
                return BadRequest(new ApiResponse<LessonSchedule>(false, "Некорректные данные."));
            }

            var existingSchedule = await _unitOfWork.LessonSchedules.GetByIdAsync(id);
            if (existingSchedule == null)
            {
                _logger.LogWarning($"Расписание с ID {id} не найдено при обновлении.");
                return NotFound(new ApiResponse<LessonSchedule>(false, "Расписание не найдено."));
            }

            // Обновляем необходимые поля
            existingSchedule.TeacherId = lessonSchedule.TeacherId;
            existingSchedule.SubjectId = lessonSchedule.SubjectId;
            existingSchedule.RoomId = lessonSchedule.RoomId;
            existingSchedule.ClassGrade = lessonSchedule.ClassGrade;
            existingSchedule.DayOfWeek = lessonSchedule.DayOfWeek;
            existingSchedule.LessonNumber = lessonSchedule.LessonNumber;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Расписание с ID {id} успешно обновлено.");
                return Ok(new ApiResponse<LessonSchedule>(true, "Расписание успешно обновлено.", existingSchedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении расписания с ID {id}.");
                return StatusCode(500, new ApiResponse<LessonSchedule>(false, "Внутренняя ошибка сервера."));
            }
        }

        /// <summary>
        /// Удаление расписания по ID.
        /// </summary>
        /// <param name="id">ID расписания.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var schedule = await _unitOfWork.LessonSchedules.GetByIdAsync(id);
            if (schedule == null)
            {
                _logger.LogWarning($"Расписание с ID {id} не найдено при удалении.");
                return NotFound(new ApiResponse<string>(false, "Расписание не найдено."));
            }

            try
            {
                _unitOfWork.LessonSchedules.Remove(schedule);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Расписание с ID {id} успешно удалено.");
                return Ok(new ApiResponse<string>(true, "Расписание успешно удалено."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении расписания с ID {id}.");
                return StatusCode(500, new ApiResponse<string>(false, "Внутренняя ошибка сервера."));
            }
        }
    }
}