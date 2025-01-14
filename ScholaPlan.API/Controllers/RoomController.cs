using Microsoft.AspNetCore.Mvc;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ScholaPlanDbContext _context;

        public RoomController(ScholaPlanDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Room room)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var school = await _context.Schools.FindAsync(room.SchoolId);
            if (school == null)
                return NotFound("Школа не найдена");

            room.School = school;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetById(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound("Кабинет не найден");
            return Ok(room);
        }
    }
}