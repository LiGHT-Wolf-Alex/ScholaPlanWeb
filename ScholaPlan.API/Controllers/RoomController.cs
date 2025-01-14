using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using System.Threading.Tasks;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Требуется авторизация для всех действий
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IUnitOfWork unitOfWork, ILogger<RoomController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Создание нового кабинета.
        /// </summary>
        /// <param name="room">Модель кабинета.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Только администраторы могут создавать кабинеты
        public async Task<ActionResult<ApiResponse<Room>>> Create([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при создании кабинета.");
                return BadRequest(new ApiResponse<Room>(false, "Некорректные данные."));
            }

            var school = await _unitOfWork.Schools.GetByIdAsync(room.SchoolId);
            if (school == null)
            {
                _logger.LogWarning($"Школа с ID {room.SchoolId} не найдена при создании кабинета.");
                return NotFound(new ApiResponse<Room>(false, "Школа не найдена."));
            }

            room.School = school;

            try
            {
                await _unitOfWork.Rooms.AddAsync(room);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Кабинет с ID {room.Id} успешно создан.");
                return CreatedAtAction(nameof(GetById), new { id = room.Id },
                    new ApiResponse<Room>(true, "Кабинет успешно создан.", room));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании кабинета.");
                return StatusCode(500, new ApiResponse<Room>(false, "Внутренняя ошибка сервера."));
            }
        }

        /// <summary>
        /// Получение кабинета по ID.
        /// </summary>
        /// <param name="id">ID кабинета.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Room>>> GetById(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning($"Кабинет с ID {id} не найден.");
                return NotFound(new ApiResponse<Room>(false, "Кабинет не найден."));
            }

            return Ok(new ApiResponse<Room>(true, "Кабинет найден.", room));
        }

        /// <summary>
        /// Обновление существующего кабинета.
        /// </summary>
        /// <param name="id">ID кабинета.</param>
        /// <param name="room">Обновленные данные кабинета.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Только администраторы могут обновлять кабинеты
        public async Task<ActionResult<ApiResponse<Room>>> Update(int id, [FromBody] Room room)
        {
            if (id != room.Id)
            {
                _logger.LogWarning($"ID кабинета в URL ({id}) не совпадает с ID в теле запроса ({room.Id}).");
                return BadRequest(new ApiResponse<Room>(false, "Несоответствие ID."));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Некорректные данные при обновлении кабинета.");
                return BadRequest(new ApiResponse<Room>(false, "Некорректные данные."));
            }

            var existingRoom = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (existingRoom == null)
            {
                _logger.LogWarning($"Кабинет с ID {id} не найден при обновлении.");
                return NotFound(new ApiResponse<Room>(false, "Кабинет не найден."));
            }

            existingRoom.Number = room.Number;
            existingRoom.Type = room.Type;
            existingRoom.SchoolId = room.SchoolId;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Кабинет с ID {id} успешно обновлен.");
                return Ok(new ApiResponse<Room>(true, "Кабинет успешно обновлен.", existingRoom));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении кабинета с ID {id}.");
                return StatusCode(500, new ApiResponse<Room>(false, "Внутренняя ошибка сервера."));
            }
        }

        /// <summary>
        /// Удаление кабинета по ID.
        /// </summary>
        /// <param name="id">ID кабинета.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Только администраторы могут удалять кабинеты
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning($"Кабинет с ID {id} не найден при удалении.");
                return NotFound(new ApiResponse<string>(false, "Кабинет не найден."));
            }

            try
            {
                _unitOfWork.Rooms.Remove(room);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Кабинет с ID {id} успешно удален.");
                return Ok(new ApiResponse<string>(true, "Кабинет успешно удален."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении кабинета с ID {id}.");
                return StatusCode(500, new ApiResponse<string>(false, "Внутренняя ошибка сервера."));
            }
        }
    }
}