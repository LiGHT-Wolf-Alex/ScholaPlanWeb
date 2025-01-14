using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Application.Interfaces.IRepositories;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы с расписанием занятий.
/// </summary>
public class LessonScheduleRepository : ILessonScheduleRepository
{
    private readonly ScholaPlanDbContext _context;

    public LessonScheduleRepository(ScholaPlanDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LessonSchedule>> GetBySchoolIdAsync(int schoolId)
    {
        return await _context.LessonSchedules
            .Where(ls => ls.SchoolId == schoolId)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<LessonSchedule> lessonSchedules)
    {
        await _context.LessonSchedules.AddRangeAsync(lessonSchedules);
    }

    public async Task DeleteBySchoolIdAsync(int schoolId)
    {
        var schedules = await GetBySchoolIdAsync(schoolId);
        _context.LessonSchedules.RemoveRange(schedules);
    }
}