using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IUnitOfWork unitOfWork, ILogger<RoomController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Room>>> Create([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при создании кабинета.");
                return BadRequest(ApiResponse<Room>.FailureResponse("Некорректные данные."));
            }

            var school = await _unitOfWork.Schools.GetByIdAsync(room.SchoolId);
            if (school == null)
            {
                _logger.LogWarning($"Школа с ID {room.SchoolId} не найдена при создании кабинета.");
                return NotFound(ApiResponse<Room>.FailureResponse("Школа не найдена."));
            }

            room.School = school;

            try
            {
                await _unitOfWork.Rooms.AddAsync(room);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Кабинет с ID {room.Id} успешно создан.");
                return CreatedAtAction(nameof(GetById), new { id = room.Id },
                    ApiResponse<Room>.SuccessResponse("Кабинет успешно создан.", room));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании кабинета.");
                return StatusCode(500, ApiResponse<Room>.FailureResponse("Внутренняя ошибка сервера."));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Room>>> GetById(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning($"Кабинет с ID {id} не найден.");
                return NotFound(ApiResponse<Room>.FailureResponse("Кабинет не найден."));
            }

            return Ok(ApiResponse<Room>.SuccessResponse("Кабинет найден.", room));
        }
    }
}