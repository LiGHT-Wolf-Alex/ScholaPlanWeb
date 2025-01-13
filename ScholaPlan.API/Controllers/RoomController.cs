using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.API.Controllers;

/// <summary>
/// Контроллер для управления кабинетами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ScholaPlanDbContext _context;

    public RoomController(ScholaPlanDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetAll()
    {
        var rooms = await _context.Rooms.AsNoTracking().ToListAsync();
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Room room)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = room.Id }, room);
    }
}