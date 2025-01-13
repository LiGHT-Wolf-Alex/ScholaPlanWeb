using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.API.Controllers
{
    /// <summary>
    /// Контроллер для управления расписанием занятий.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScholaPlanDbContext _context;

        /// <summary>
        /// Конструктор контроллера с внедрением зависимости контекста базы данных.
        /// </summary>
        public ScheduleController(ScholaPlanDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все записи расписания.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonSchedule>>> GetAll()
        {
            var schedules = await _context.LessonSchedules
                .Include(ls => ls.Teacher)
                .Include(ls => ls.Subject)
                .Include(ls => ls.Room)
                .ToListAsync();
            return Ok(schedules);
        }

        /// <summary>
        /// Получить расписание по идентификатору.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonSchedule>> GetById(int id)
        {
            var schedule = await _context.LessonSchedules
                .Include(ls => ls.Teacher)
                .Include(ls => ls.Subject)
                .Include(ls => ls.Room)
                .FirstOrDefaultAsync(ls => ls.Id == id);

            if (schedule == null)
                return NotFound("Запись расписания не найдена.");

            return Ok(schedule);
        }

        /// <summary>
        /// Добавить новую запись в расписание.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] LessonSchedule schedule)
        {
            _context.LessonSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
        }

        /// <summary>
        /// Обновить существующую запись расписания.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] LessonSchedule updatedSchedule)
        {
            if (id != updatedSchedule.Id)
                return BadRequest("Идентификатор не совпадает.");

            var existingSchedule = await _context.LessonSchedules.FindAsync(id);
            if (existingSchedule == null)
                return NotFound("Запись расписания не найдена.");

            _context.Entry(existingSchedule).CurrentValues.SetValues(updatedSchedule);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Удалить запись расписания по идентификатору.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var schedule = await _context.LessonSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Запись расписания не найдена.");

            _context.LessonSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}