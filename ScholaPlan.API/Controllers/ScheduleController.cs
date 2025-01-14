using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IScheduleService scheduleService, IUnitOfWork unitOfWork,
            ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<ApiResponse<GenerateScheduleResponse>>> GenerateSchedule(
            [FromBody] GenerateScheduleRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при генерации расписания.");
                return BadRequest(ApiResponse<GenerateScheduleResponse>.FailureResponse("Некорректные данные."));
            }

            var school = await _unitOfWork.Schools.GetByIdAsync(request.SchoolId);
            if (school == null)
            {
                _logger.LogWarning($"Школа с ID {request.SchoolId} не найдена при генерации расписания.");
                return NotFound(ApiResponse<GenerateScheduleResponse>.FailureResponse("Школа не найдена."));
            }

            try
            {
                var schedule = _scheduleService.GenerateSchedule(school);
                await _unitOfWork.LessonSchedules.AddRangeAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation(
                    $"Расписание для школы ID {request.SchoolId} успешно сгенерировано и сохранено.");
                return Ok(ApiResponse<GenerateScheduleResponse>.SuccessResponse(
                    "Расписание успешно сгенерировано и сохранено.",
                    new GenerateScheduleResponse
                        { Success = true, Message = "Расписание успешно сгенерировано и сохранено." }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации расписания.");
                return StatusCode(500,
                    ApiResponse<GenerateScheduleResponse>.FailureResponse("Ошибка сервера при генерации расписания."));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LessonSchedule>>> GetScheduleById(int id)
        {
            var schedule = await _unitOfWork.LessonSchedules.GetByIdAsync(id);
            if (schedule == null)
            {
                _logger.LogWarning($"Расписание с ID {id} не найдено.");
                return NotFound(ApiResponse<LessonSchedule>.FailureResponse("Расписание не найдено."));
            }

            return Ok(ApiResponse<LessonSchedule>.SuccessResponse("Расписание найдено.", schedule));
        }
    }
}