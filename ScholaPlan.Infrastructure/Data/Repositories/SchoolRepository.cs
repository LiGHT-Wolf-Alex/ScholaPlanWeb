using Microsoft.EntityFrameworkCore;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы со школами.
/// </summary>
public class SchoolRepository(ScholaPlanDbContext context) : ISchoolRepository
{
    public async Task<School?> GetByIdAsync(int schoolId)
    {
        return await context.Schools
            .Include(s => s.Teachers)
            .Include(s => s.Subjects)
            .Include(s => s.Rooms)
            .Include(s => s.MaxLessonsPerDayConfigs)
            .FirstOrDefaultAsync(s => s.Id == schoolId);
    }

    public async Task<List<School>> GetAllAsync()
    {
        return await context.Schools
            .Include(s => s.Teachers)
            .Include(s => s.Subjects)
            .Include(s => s.Rooms)
            .Include(s => s.MaxLessonsPerDayConfigs)
            .ToListAsync();
    }

    public async Task AddAsync(School school)
    {
        await context.Schools.AddAsync(school);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}