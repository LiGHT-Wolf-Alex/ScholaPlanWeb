using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Application.Interfaces.IRepositories;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ISchoolRepository _schoolRepository;

        public ScheduleController(IScheduleService scheduleService, ISchoolRepository schoolRepository)
        {
            _scheduleService = scheduleService;
            _schoolRepository = schoolRepository;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSchedule([FromBody] GenerateScheduleRequest request)
        {
            var school = await _schoolRepository.GetByIdAsync(request.SchoolId);
            if (school == null)
            {
                return NotFound("Ошибка сервера: Школа не найдена");
            }

            try
            {
                var schedule = _scheduleService.GenerateSchedule(school);
                return Ok("Расписание успешно сгенерировано.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
    }
}