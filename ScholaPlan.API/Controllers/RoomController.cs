
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

    /// <summary>
    /// Получить все кабинеты.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetAll()
    {
        var rooms = await _context.Rooms.ToListAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// Получить кабинет по идентификатору.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetById(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound("Кабинет не найден.");
        return Ok(room);
    }

    /// <summary>
    /// Добавить новый кабинет.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    /// <summary>
    /// Обновить данные кабинета.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Room updatedRoom)
    {
        if (id != updatedRoom.Id)
            return BadRequest("Идентификатор не совпадает.");

        var existingRoom = await _context.Rooms.FindAsync(id);
        if (existingRoom == null)
            return NotFound("Кабинет не найден.");

        _context.Entry(existingRoom).CurrentValues.SetValues(updatedRoom);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Удалить кабинет.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFound("Кабинет не найден.");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}