using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с учителями.
    /// </summary>
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ScholaPlanDbContext _context;

        public TeacherRepository(ScholaPlanDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetByIdAsync(int teacherId)
        {
            return await _context.Teachers
                .Include(t => t.School)
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
        }

        // Дополнительные методы при необходимости
    }
}