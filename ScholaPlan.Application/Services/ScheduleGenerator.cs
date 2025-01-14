using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;

namespace ScholaPlan.Application.Services
{
    public class ScheduleGenerator : IScheduleGenerator
    {
        private readonly IEnumerable<DayOfWeek> _daysOfWeek = new[]
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        public IEnumerable<LessonSchedule> GenerateSchedule(School school)
        {
            var generatedSchedules = new List<LessonSchedule>();

            // Доступность учителей и кабинетов: (DayOfWeek, lessonNumber) -> Teacher/Room
            var teacherAvailability = new Dictionary<Teacher, HashSet<(DayOfWeek, int)>>();
            var roomAvailability = new Dictionary<Room, HashSet<(DayOfWeek, int)>>();

            foreach (var teacher in school.Teachers)
                teacherAvailability[teacher] = new HashSet<(DayOfWeek, int)>();

            foreach (var room in school.Rooms)
                roomAvailability[room] = new HashSet<(DayOfWeek, int)>();

            // Счётчик уроков для каждого класса и дня
            // Ключ: (classGrade, DayOfWeek), Значение: уже запланированное количество уроков
            var lessonsCountByClassDay = new Dictionary<(int, DayOfWeek), int>();

            // Идём по каждому классу, для которого есть MaxLessonsPerDayConfig
            foreach (var classConfig in school.MaxLessonsPerDayConfigs)
            {
                int classGrade = classConfig.ClassGrade;
                int maxLessonsPerDay = classConfig.MaxLessons;

                // Планируем каждый предмет в этом классе
                foreach (var subject in school.Subjects)
                {
                    for (int i = 0; i < subject.WeeklyHours; i++)
                    {
                        // Ищем подходящего учителя
                        var availableTeachers = school.Teachers
                            .Where(t => t.Specializations.Contains(subject.Specialization))
                            .ToList();
                        if (!availableTeachers.Any())
                            throw new InvalidOperationException(
                                $"Нет доступных учителей для предмета {subject.Name}");

                        // Ищем доступные комнаты (допустим, под этот предмет подходит Standard)
                        var availableRooms = school.Rooms
                            .Where(r => r.Type == RoomType.Standard)
                            .ToList();
                        if (!availableRooms.Any())
                            throw new InvalidOperationException(
                                $"Нет доступных кабинетов для предмета {subject.Name}");

                        bool scheduled = false;

                        // Перебираем дни и номера уроков
                        foreach (var day in _daysOfWeek)
                        {
                            for (int lessonNumber = 1; lessonNumber <= 8; lessonNumber++)
                            {
                                // Проверяем, не превышен ли лимит уроков в день для этого класса
                                var key = (classGrade, day);
                                lessonsCountByClassDay.TryGetValue(key, out int currentCount);
                                if (currentCount >= maxLessonsPerDay)
                                {
                                    // Уже достигли лимита уроков для этого дня
                                    continue;
                                }

                                // Найти свободного учителя
                                var teacher = availableTeachers.FirstOrDefault(t =>
                                    !teacherAvailability[t].Contains((day, lessonNumber)));

                                if (teacher == null)
                                    continue;

                                // Найти свободную комнату
                                var room = availableRooms.FirstOrDefault(r =>
                                    !roomAvailability[r].Contains((day, lessonNumber)));

                                if (room == null)
                                    continue;

                                // Создаём LessonSchedule
                                var lessonSchedule = new LessonSchedule
                                {
                                    School = school,
                                    SchoolId = school.Id,

                                    Teacher = teacher,
                                    TeacherId = teacher.Id,

                                    Subject = subject,
                                    SubjectId = subject.Id,

                                    Room = room,
                                    RoomId = room.Id,

                                    ClassGrade = classGrade, // Класс из classConfig
                                    DayOfWeek = day,
                                    LessonNumber = lessonNumber
                                };

                                generatedSchedules.Add(lessonSchedule);

                                // Обновляем занятость учителя/комнаты
                                teacherAvailability[teacher].Add((day, lessonNumber));
                                roomAvailability[room].Add((day, lessonNumber));

                                // Увеличиваем счётчик уроков для этого (classGrade, day)
                                lessonsCountByClassDay[key] = currentCount + 1;

                                scheduled = true;
                                break; // Ушли на следующий час по этому предмету
                            }

                            if (scheduled)
                                break; // Переходим к следующему уроку subject.WeeklyHours
                        }

                        if (!scheduled)
                        {
                            throw new InvalidOperationException(
                                $"Не удалось назначить урок (Class={classGrade}, Subject={subject.Name}). " +
                                "Недостаточно ресурсов или превышен лимит уроков.");
                        }
                    }
                }
            }

            return generatedSchedules;
        }
    }
}