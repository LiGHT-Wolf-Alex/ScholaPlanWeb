using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы с предметами.
/// </summary>
public class SubjectRepository(ScholaPlanDbContext context, ILogger<SubjectRepository> logger)
    : ISubjectRepository
{
    public async Task<Subject?> GetByIdAsync(int subjectId)
    {
        try
        {
            logger.LogInformation($"Получение предмета с ID {subjectId}.");
            return await context.Subjects
                .FirstOrDefaultAsync(s => s.Id == subjectId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при получении предмета с ID {subjectId}.");
            throw;
        }
    }

    public async Task AddAsync(Subject subject)
    {
        try
        {
            logger.LogInformation($"Добавление нового предмета: {subject.Name}.");
            await context.Subjects.AddAsync(subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при добавлении предмета.");
            throw;
        }
    }

    public void Remove(Subject subject)
    {
        try
        {
            logger.LogInformation($"Удаление предмета с ID {subject.Id}.");
            context.Subjects.Remove(subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при удалении предмета с ID {subject.Id}.");
            throw;
        }
    }
}