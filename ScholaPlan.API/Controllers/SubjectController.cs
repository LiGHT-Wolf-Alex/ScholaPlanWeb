using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(IUnitOfWork unitOfWork, ILogger<SubjectController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Subject>>> Create([FromBody] Subject subject)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при создании предмета.");
                return BadRequest(ApiResponse<Subject>.FailureResponse("Некорректные данные."));
            }

            try
            {
                await _unitOfWork.Subjects.AddAsync(subject);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Предмет с ID {subject.Id} успешно создан.");
                return CreatedAtAction(nameof(GetById), new { id = subject.Id },
                    ApiResponse<Subject>.SuccessResponse("Предмет успешно создан.", subject));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании предмета.");
                return StatusCode(500, ApiResponse<Subject>.FailureResponse("Внутренняя ошибка сервера."));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Subject>>> GetById(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
            if (subject == null)
            {
                _logger.LogWarning($"Предмет с ID {id} не найден.");
                return NotFound(ApiResponse<Subject>.FailureResponse("Предмет не найден."));
            }

            return Ok(ApiResponse<Subject>.SuccessResponse("Предмет найден.", subject));
        }
    }
}