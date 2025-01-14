using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с учителями.
    /// </summary>
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ScholaPlanDbContext _context;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(ScholaPlanDbContext context, ILogger<TeacherRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Teacher?> GetByIdAsync(int teacherId)
        {
            try
            {
                _logger.LogInformation($"Получение учителя с ID {teacherId}.");
                return await _context.Teachers
                    .Include(t => t.School)
                    .Include(t => t.Lessons)
                    .FirstOrDefaultAsync(t => t.Id == teacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении учителя с ID {teacherId}.");
                throw;
            }
        }

        public async Task AddAsync(Teacher teacher)
        {
            try
            {
                _logger.LogInformation($"Добавление нового учителя: {teacher.Name.FirstName} {teacher.Name.LastName}.");
                await _context.Teachers.AddAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении учителя.");
                throw;
            }
        }

        public void Remove(Teacher teacher)
        {
            try
            {
                _logger.LogInformation($"Удаление учителя с ID {teacher.Id}.");
                _context.Teachers.Remove(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении учителя с ID {teacher.Id}.");
                throw;
            }
        }

        // Дополнительные методы при необходимости
    }
}