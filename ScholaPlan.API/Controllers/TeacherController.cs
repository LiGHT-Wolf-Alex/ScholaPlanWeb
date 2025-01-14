using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(IUnitOfWork unitOfWork, ILogger<TeacherController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Teacher>>> Create([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при создании учителя.");
                return BadRequest(ApiResponse<Teacher>.FailureResponse("Некорректные данные."));
            }

            var school = await _unitOfWork.Schools.GetByIdAsync(teacher.SchoolId);
            if (school == null)
            {
                _logger.LogWarning($"Школа с ID {teacher.SchoolId} не найдена при создании учителя.");
                return NotFound(ApiResponse<Teacher>.FailureResponse("Школа не найдена."));
            }

            teacher.School = school;

            try
            {
                await _unitOfWork.Teachers.AddAsync(teacher);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Учитель с ID {teacher.Id} успешно создан.");
                return CreatedAtAction(nameof(GetById), new { id = teacher.Id },
                    ApiResponse<Teacher>.SuccessResponse("Учитель успешно создан.", teacher));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании учителя.");
                return StatusCode(500, ApiResponse<Teacher>.FailureResponse("Внутренняя ошибка сервера."));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Teacher>>> GetById(int id)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher == null)
            {
                _logger.LogWarning($"Учитель с ID {id} не найден.");
                return NotFound(ApiResponse<Teacher>.FailureResponse("Учитель не найден."));
            }

            return Ok(ApiResponse<Teacher>.SuccessResponse("Учитель найден.", teacher));
        }
    }
}