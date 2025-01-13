using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;

/// <summary>
/// Генерация расписания для указанной школы.
/// </summary>
/// <param name="school">Школа, для которой генерируется расписание.</param>
/// <returns>Сгенерированное расписание для школы.</returns>
public class ScheduleGenerator : IScheduleGenerator
{
    public IEnumerable<LessonSchedule> GenerateSchedule(School school)
    {
        var generatedSchedules = new List<LessonSchedule>();

        foreach (var subject in school.Subjects)
        {
            for (int i = 0; i < subject.WeeklyHours; i++)
            {
                var availableTeachers = school.Teachers
                    .Where(t => t.Specializations.Any(s => s == subject.Specialization));

                if (!availableTeachers.Any())
                    throw new InvalidOperationException($"Нет доступных учителей для предмета {subject.Name}");

                var availableRooms = school.Rooms
                    .Where(r => r.Type == RoomType.Standard);

                if (!availableRooms.Any())
                    throw new InvalidOperationException($"Нет доступных кабинетов для предмета {subject.Name}");
            }
        }

        return generatedSchedules;
    }
}