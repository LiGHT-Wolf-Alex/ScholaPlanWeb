using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.API.Controllers;

/// <summary>
/// Контроллер для управления предметами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SubjectController : ControllerBase
{
    private readonly ScholaPlanDbContext _context;

    public SubjectController(ScholaPlanDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все предметы.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subject>>> GetAll()
    {
        var subjects = await _context.Subjects.ToListAsync();
        return Ok(subjects);
    }

    /// <summary>
    /// Получить предмет по идентификатору.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Subject>> GetById(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
            return NotFound("Предмет не найден.");
        return Ok(subject);
    }

    /// <summary>
    /// Добавить новый предмет.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Subject subject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }

    /// <summary>
    /// Обновить существующий предмет.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Subject updatedSubject)
    {
        if (id != updatedSubject.Id)
            return BadRequest("Идентификатор не совпадает.");

        var existingSubject = await _context.Subjects.FindAsync(id);
        if (existingSubject == null)
            return NotFound("Предмет не найден.");

        _context.Entry(existingSubject).CurrentValues.SetValues(updatedSubject);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Удалить предмет по идентификатору.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
            return NotFound("Предмет не найден.");

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}