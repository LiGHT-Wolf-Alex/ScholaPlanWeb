// ScholaPlan.API.Controllers.ScheduleController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSchedule([FromBody] ScheduleRequest request)
        {
            var schedules = await _scheduleService.GenerateScheduleAsync(request.School, request.TeacherPreferences);
            return Ok(schedules);
        }
    }

    public class ScheduleRequest
    {
        public School School { get; set; }
        public Dictionary<int, TeacherPreferences> TeacherPreferences { get; set; }
    }
}