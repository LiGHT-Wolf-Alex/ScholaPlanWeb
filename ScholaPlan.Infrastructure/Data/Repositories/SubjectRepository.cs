using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с предметами.
    /// </summary>
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ScholaPlanDbContext _context;
        private readonly ILogger<SubjectRepository> _logger;

        public SubjectRepository(ScholaPlanDbContext context, ILogger<SubjectRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Subject?> GetByIdAsync(int subjectId)
        {
            try
            {
                _logger.LogInformation($"Получение предмета с ID {subjectId}.");
                return await _context.Subjects
                    .FirstOrDefaultAsync(s => s.Id == subjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении предмета с ID {subjectId}.");
                throw;
            }
        }

        public async Task AddAsync(Subject subject)
        {
            try
            {
                _logger.LogInformation($"Добавление нового предмета: {subject.Name}.");
                await _context.Subjects.AddAsync(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении предмета.");
                throw;
            }
        }

        public void Remove(Subject subject)
        {
            try
            {
                _logger.LogInformation($"Удаление предмета с ID {subject.Id}.");
                _context.Subjects.Remove(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении предмета с ID {subject.Id}.");
                throw;
            }
        }
    }
}