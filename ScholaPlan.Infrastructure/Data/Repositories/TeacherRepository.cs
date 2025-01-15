using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы с учителями.
/// </summary>
public class TeacherRepository(ScholaPlanDbContext context, ILogger<TeacherRepository> logger)
    : ITeacherRepository
{
    public async Task<Teacher?> GetByIdAsync(int teacherId)
    {
        try
        {
            logger.LogInformation($"Получение учителя с ID {teacherId}.");
            return await context.Teachers
                .Include(t => t.School)
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(t => t.Id == teacherId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при получении учителя с ID {teacherId}.");
            throw;
        }
    }

    public async Task AddAsync(Teacher teacher)
    {
        try
        {
            logger.LogInformation($"Добавление нового учителя: {teacher.Name.FirstName} {teacher.Name.LastName}.");
            await context.Teachers.AddAsync(teacher);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при добавлении учителя.");
            throw;
        }
    }

    public void Remove(Teacher teacher)
    {
        try
        {
            logger.LogInformation($"Удаление учителя с ID {teacher.Id}.");
            context.Teachers.Remove(teacher);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при удалении учителя с ID {teacher.Id}.");
            throw;
        }
    }

    // Дополнительные методы при необходимости
}