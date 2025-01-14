using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с предметами.
    /// </summary>
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ScholaPlanDbContext _context;

        public SubjectRepository(ScholaPlanDbContext context)
        {
            _context = context;
        }

        public async Task<Subject?> GetByIdAsync(int subjectId)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == subjectId);
        }

        public async Task AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
        }

        // Дополнительные методы при необходимости
    }
}