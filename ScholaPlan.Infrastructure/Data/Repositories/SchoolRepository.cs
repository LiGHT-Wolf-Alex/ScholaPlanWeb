using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы со школами.
    /// </summary>
    public class SchoolRepository : ISchoolRepository
    {
        private readonly ScholaPlanDbContext _context;

        public SchoolRepository(ScholaPlanDbContext context)
        {
            _context = context;
        }

        public async Task<School?> GetByIdAsync(int schoolId)
        {
            return await _context.Schools
                .Include(s => s.Teachers)
                .Include(s => s.Subjects)
                .Include(s => s.Rooms)
                .Include(s => s.MaxLessonsPerDayConfigs)
                .FirstOrDefaultAsync(s => s.Id == schoolId);
        }

        public async Task<List<School>> GetAllAsync()
        {
            return await _context.Schools
                .Include(s => s.Teachers)
                .Include(s => s.Subjects)
                .Include(s => s.Rooms)
                .Include(s => s.MaxLessonsPerDayConfigs)
                .ToListAsync();
        }

        public async Task AddAsync(School school)
        {
            await _context.Schools.AddAsync(school);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}