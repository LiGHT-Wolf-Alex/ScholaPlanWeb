using Microsoft.AspNetCore.Mvc;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ScholaPlanDbContext _context;

        public TeacherController(ScholaPlanDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var school = await _context.Schools.FindAsync(teacher.SchoolId);
            if (school == null)
                return NotFound("Школа не найдена");

            teacher.School = school;
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetById(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound("Учитель не найден");
            return Ok(teacher);
        }
    }
}