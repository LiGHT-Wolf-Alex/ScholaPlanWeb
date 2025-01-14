using Microsoft.AspNetCore.Mvc;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ScholaPlanDbContext _context;

        public SubjectController(ScholaPlanDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Subject subject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetById(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound("Предмет не найден");
            return Ok(subject);
        }
    }
}