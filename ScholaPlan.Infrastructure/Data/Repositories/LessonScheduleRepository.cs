using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ScholaPlan.Infrastructure.Repositories
{
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

        public async Task<LessonSchedule?> GetByIdAsync(int scheduleId)
        {
            return await _context.LessonSchedules
                .Include(ls => ls.School)
                .Include(ls => ls.Teacher)
                .Include(ls => ls.Subject)
                .Include(ls => ls.Room)
                .FirstOrDefaultAsync(ls => ls.Id == scheduleId);
        }

        public async Task<IEnumerable<LessonSchedule>> GetBySchoolIdAsync(int schoolId)
        {
            return await _context.LessonSchedules
                .Where(ls => ls.SchoolId == schoolId)
                .Include(ls => ls.Teacher)
                .Include(ls => ls.Subject)
                .Include(ls => ls.Room)
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
}