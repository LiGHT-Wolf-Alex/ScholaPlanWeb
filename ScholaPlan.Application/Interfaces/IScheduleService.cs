using ScholaPlan.Domain.Entities;

public interface IScheduleService
{
    /// <summary>
    /// Генерация расписания для указанной школы.
    /// </summary>
    /// <param name="school">Школа, для которой создается расписание.</param>
    /// <returns>Сгенерированное расписание.</returns>
    List<LessonSchedule> GenerateSchedule(School school);
}