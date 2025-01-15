using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Требуется авторизация для всех действий
public class TeacherController(IUnitOfWork unitOfWork, ILogger<TeacherController> logger) : ControllerBase
{
    /// <summary>
    /// Создание нового учителя.
    /// </summary>
    /// <param name="teacher">Модель учителя.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Teacher>>> Create([FromBody] Teacher teacher)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Некорректные данные при создании учителя.");
            return BadRequest(new ApiResponse<Teacher>(false, "Некорректные данные."));
        }

        var school = await unitOfWork.Schools.GetByIdAsync(teacher.SchoolId);
        if (school == null)
        {
            logger.LogWarning($"Школа с ID {teacher.SchoolId} не найдена при создании учителя.");
            return NotFound(new ApiResponse<Teacher>(false, "Школа не найдена."));
        }

        teacher.School = school;

        try
        {
            await unitOfWork.Teachers.AddAsync(teacher);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation($"Учитель с ID {teacher.Id} успешно создан.");
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, new ApiResponse<Teacher>(true, "Учитель успешно создан.", teacher));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при создании учителя.");
            return StatusCode(500, new ApiResponse<Teacher>(false, "Внутренняя ошибка сервера."));
        }
    }

    /// <summary>
    /// Получение учителя по ID.
    /// </summary>
    /// <param name="id">ID учителя.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Teacher>>> GetById(int id)
    {
        var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher == null)
        {
            logger.LogWarning($"Учитель с ID {id} не найден.");
            return NotFound(new ApiResponse<Teacher>(false, "Учитель не найден."));
        }
        return Ok(new ApiResponse<Teacher>(true, "Учитель найден.", teacher));
    }

    /// <summary>
    /// Обновление существующего учителя.
    /// </summary>
    /// <param name="id">ID учителя.</param>
    /// <param name="teacher">Обновленные данные учителя.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Teacher>>> Update(int id, [FromBody] Teacher teacher)
    {
        if (id != teacher.Id)
        {
            logger.LogWarning($"ID учителя в URL ({id}) не совпадает с ID в теле запроса ({teacher.Id}).");
            return BadRequest(new ApiResponse<Teacher>(false, "Несоответствие ID."));
        }

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Некорректные данные при обновлении учителя.");
            return BadRequest(new ApiResponse<Teacher>(false, "Некорректные данные."));
        }

        var existingTeacher = await unitOfWork.Teachers.GetByIdAsync(id);
        if (existingTeacher == null)
        {
            logger.LogWarning($"Учитель с ID {id} не найден при обновлении.");
            return NotFound(new ApiResponse<Teacher>(false, "Учитель не найден."));
        }

        existingTeacher.Name = teacher.Name;
        existingTeacher.Specializations = teacher.Specializations;
        existingTeacher.SchoolId = teacher.SchoolId;

        try
        {
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation($"Учитель с ID {id} успешно обновлен.");
            return Ok(new ApiResponse<Teacher>(true, "Учитель успешно обновлен.", existingTeacher));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при обновлении учителя с ID {id}.");
            return StatusCode(500, new ApiResponse<Teacher>(false, "Внутренняя ошибка сервера."));
        }
    }

    /// <summary>
    /// Удаление учителя по ID.
    /// </summary>
    /// <param name="id">ID учителя.</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var teacher = await unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher == null)
        {
            logger.LogWarning($"Учитель с ID {id} не найден при удалении.");
            return NotFound(new ApiResponse<string>(false, "Учитель не найден."));
        }

        try
        {
            unitOfWork.Teachers.Remove(teacher);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation($"Учитель с ID {id} успешно удален.");
            return Ok(new ApiResponse<string>(true, "Учитель успешно удален."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при удалении учителя с ID {id}.");
            return StatusCode(500, new ApiResponse<string>(false, "Внутренняя ошибка сервера."));
        }
    }
}