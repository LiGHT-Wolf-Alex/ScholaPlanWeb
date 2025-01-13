using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;


namespace ScholaPlan.API.Controllers;

/// <summary>
/// Контроллер для управления учителями.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ScholaPlanDbContext _context;

    public TeacherController(ScholaPlanDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить всех учителей.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetAll()
    {
        var teachers = await _context.Teachers.ToListAsync();
        return Ok(teachers);
    }

    /// <summary>
    /// Получить учителя по идентификатору.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Teacher>> GetById(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
            return NotFound("Учитель не найден.");
        return Ok(teacher);
    }

    /// <summary>
    /// Добавить нового учителя.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Teacher teacher)
    {
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
    }

    /// <summary>
    /// Обновить данные учителя.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Teacher updatedTeacher)
    {
        if (id != updatedTeacher.Id)
            return BadRequest("Идентификатор не совпадает.");

        var existingTeacher = await _context.Teachers.FindAsync(id);
        if (existingTeacher == null)
            return NotFound("Учитель не найден.");

        _context.Entry(existingTeacher).CurrentValues.SetValues(updatedTeacher);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Удалить учителя.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
            return NotFound("Учитель не найден.");

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}