using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces;

/// <summary>
/// Интерфейс генератора расписания.
/// </summary>
public interface IScheduleGenerator
{
    Task<IEnumerable<LessonSchedule>> GenerateScheduleAsync(School school,
        Dictionary<int, TeacherPreferences> teacherPreferences);
}