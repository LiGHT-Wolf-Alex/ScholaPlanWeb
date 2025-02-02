﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Требуется авторизация для всех действий
public class SubjectController(IUnitOfWork unitOfWork, ILogger<SubjectController> logger) : ControllerBase
{
    /// <summary>
    /// Создание нового предмета.
    /// </summary>
    /// <param name="subject">Модель предмета.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Subject>>> Create([FromBody] Subject subject)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Некорректные данные при создании предмета.");
            return BadRequest(new ApiResponse<Subject>(false, "Некорректные данные."));
        }

        try
        {
            await unitOfWork.Subjects.AddAsync(subject);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation($"Предмет с ID {subject.Id} успешно создан.");
            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, new ApiResponse<Subject>(true, "Предмет успешно создан.", subject));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при создании предмета.");
            return StatusCode(500, new ApiResponse<Subject>(false, "Внутренняя ошибка сервера."));
        }
    }

    /// <summary>
    /// Получение предмета по ID.
    /// </summary>
    /// <param name="id">ID предмета.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Subject>>> GetById(int id)
    {
        var subject = await unitOfWork.Subjects.GetByIdAsync(id);
        if (subject == null)
        {
            logger.LogWarning($"Предмет с ID {id} не найден.");
            return NotFound(new ApiResponse<Subject>(false, "Предмет не найден."));
        }
        return Ok(new ApiResponse<Subject>(true, "Предмет найден.", subject));
    }

    /// <summary>
    /// Обновление существующего предмета.
    /// </summary>
    /// <param name="id">ID предмета.</param>
    /// <param name="subject">Обновленные данные предмета.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Subject>>> Update(int id, [FromBody] Subject subject)
    {
        if (id != subject.Id)
        {
            logger.LogWarning($"ID предмета в URL ({id}) не совпадает с ID в теле запроса ({subject.Id}).");
            return BadRequest(new ApiResponse<Subject>(false, "Несоответствие ID."));
        }

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Некорректные данные при обновлении предмета.");
            return BadRequest(new ApiResponse<Subject>(false, "Некорректные данные."));
        }

        var existingSubject = await unitOfWork.Subjects.GetByIdAsync(id);
        if (existingSubject == null)
        {
            logger.LogWarning($"Предмет с ID {id} не найден при обновлении.");
            return NotFound(new ApiResponse<Subject>(false, "Предмет не найден."));
        }

        existingSubject.Name = subject.Name;
        existingSubject.DifficultyLevel = subject.DifficultyLevel;
        existingSubject.WeeklyHours = subject.WeeklyHours;
        existingSubject.Specialization = subject.Specialization;

        try
        {
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation($"Предмет с ID {id} успешно обновлен.");
            return Ok(new ApiResponse<Subject>(true, "Предмет успешно обновлен.", existingSubject));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при обновлении предмета с ID {id}.");
            return StatusCode(500, new ApiResponse<Subject>(false, "Внутренняя ошибка сервера."));
        }
    }

    /// <summary>
    /// Удаление предмета по ID.
    /// </summary>
    /// <param name="id">ID предмета.</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var subject = await unitOfWork.Subjects.GetByIdAsync(id);
        if (subject == null)
        {
            logger.LogWarning($"Предмет с ID {id} не найден при удалении.");
            return NotFound(new ApiResponse<string>(false, "Предмет не найден."));
        }

        try
        {
            unitOfWork.Subjects.Remove(subject);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation($"Предмет с ID {id} успешно удален.");
            return Ok(new ApiResponse<string>(true, "Предмет успешно удален."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при удалении предмета с ID {id}.");
            return StatusCode(500, new ApiResponse<string>(false, "Внутренняя ошибка сервера."));
        }
    }
}