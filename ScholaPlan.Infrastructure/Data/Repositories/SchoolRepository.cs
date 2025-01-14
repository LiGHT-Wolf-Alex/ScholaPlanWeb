using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Application.Interfaces.IRepositories;

namespace ScholaPlan.Infrastructure.Repositories
{
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
                .FirstOrDefaultAsync(s => s.Id == schoolId);
        }

        public async Task<List<School>> GetAllAsync()
        {
            return await _context.Schools.ToListAsync();
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