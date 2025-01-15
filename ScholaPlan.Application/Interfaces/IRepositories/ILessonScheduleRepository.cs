using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces.IRepositories;

/// <summary>
/// Репозиторий для работы с расписанием занятий.
/// </summary>
public interface ILessonScheduleRepository
{
    Task<LessonSchedule?> GetByIdAsync(int scheduleId);
    Task<IEnumerable<LessonSchedule>> GetBySchoolIdAsync(int schoolId);
    Task AddRangeAsync(IEnumerable<LessonSchedule> lessonSchedules);
    Task DeleteBySchoolIdAsync(int schoolId);
    void Remove(LessonSchedule lessonSchedule);
}