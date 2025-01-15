using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces;

/// <summary>
/// Сервис для генерации и управления расписанием.
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// Генерирует расписание для указанной школы с учетом предпочтений учителей.
    /// </summary>
    /// <param name="school">Школа для генерации расписания.</param>
    /// <param name="teacherPreferences">Предпочтения учителей.</param>
    /// <returns>Список расписаний занятий.</returns>
    Task<List<LessonSchedule>> GenerateScheduleAsync(School school,
        Dictionary<int, TeacherPreferences> teacherPreferences);
}