using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.API.Controllers;

/// <summary>
/// Контроллер для управления расписанием занятий.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ScholaPlanDbContext _context;

    public ScheduleController(ScholaPlanDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonSchedule>>> GetAll()
    {
        var schedules = await _context.LessonSchedules
            .Include(ls => ls.Teacher)
            .Include(ls => ls.Subject)
            .Include(ls => ls.Room)
            .AsNoTracking()
            .ToListAsync();
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] LessonSchedule schedule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.LessonSchedules.Add(schedule);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = schedule.Id }, schedule);
    }
}