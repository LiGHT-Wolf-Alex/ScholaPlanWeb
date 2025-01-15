using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы с расписанием занятий.
/// </summary>
public class LessonScheduleRepository : ILessonScheduleRepository
{
    private readonly ScholaPlanDbContext _context;
    private readonly ILogger<LessonScheduleRepository> _logger;

    public LessonScheduleRepository(ScholaPlanDbContext context, ILogger<LessonScheduleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LessonSchedule?> GetByIdAsync(int scheduleId)
    {
        try
        {
            _logger.LogInformation($"Получение расписания с ID {scheduleId}.");
            return await _context.LessonSchedules
                .Include(ls => ls.School)
                .Include(ls => ls.Teacher)
                .Include(ls => ls.Subject)
                .Include(ls => ls.Room)
                .FirstOrDefaultAsync(ls => ls.Id == scheduleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при получении расписания с ID {scheduleId}.");
            throw;
        }
    }

    public async Task<IEnumerable<LessonSchedule>> GetBySchoolIdAsync(int schoolId)
    {
        try
        {
            _logger.LogInformation($"Получение расписаний для школы с ID {schoolId}.");
            return await _context.LessonSchedules
                .Where(ls => ls.SchoolId == schoolId)
                .Include(ls => ls.Teacher)
                .Include(ls => ls.Subject)
                .Include(ls => ls.Room)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при получении расписаний для школы с ID {schoolId}.");
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<LessonSchedule> lessonSchedules)
    {
        try
        {
            _logger.LogInformation($"Добавление {lessonSchedules.Count()} расписаний.");
            await _context.LessonSchedules.AddRangeAsync(lessonSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении расписаний.");
            throw;
        }
    }

    public async Task DeleteBySchoolIdAsync(int schoolId)
    {
        try
        {
            var schedules = await GetBySchoolIdAsync(schoolId);
            _context.LessonSchedules.RemoveRange(schedules);
            _logger.LogInformation($"Удаление расписаний для школы с ID {schoolId}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при удалении расписаний для школы с ID {schoolId}.");
            throw;
        }
    }

    public void Remove(LessonSchedule lessonSchedule)
    {
        try
        {
            _logger.LogInformation($"Удаление расписания с ID {lessonSchedule.Id}.");
            _context.LessonSchedules.Remove(lessonSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при удалении расписания с ID {lessonSchedule.Id}.");
            throw;
        }
    }
}