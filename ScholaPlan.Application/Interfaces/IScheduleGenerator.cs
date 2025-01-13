using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces;

public interface IScheduleGenerator
{
    /// <summary>
    /// Генерирует расписание для указанной школы.
    /// </summary>
    /// <param name="school">Школа для генерации расписания.</param>
    /// <returns>Список занятий в расписании.</returns>
    IEnumerable<LessonSchedule> GenerateSchedule(School school);
}