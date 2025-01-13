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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetAll()
    {
        var teachers = await _context.Teachers.AsNoTracking().ToListAsync();
        return Ok(teachers);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Teacher teacher)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = teacher.Id }, teacher);
    }
}