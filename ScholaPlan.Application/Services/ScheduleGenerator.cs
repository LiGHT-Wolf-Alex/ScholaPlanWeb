using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;

namespace ScholaPlan.Application.Services
{
    public class ScheduleGenerator : IScheduleGenerator
    {
        private readonly IEnumerable<DayOfWeek> _daysOfWeek = new[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday
        };

        public IEnumerable<LessonSchedule> GenerateSchedule(School school)
        {
            var generatedSchedules = new List<LessonSchedule>();
            var teacherAvailability = new Dictionary<Teacher, HashSet<(DayOfWeek, int)>>();
            var roomAvailability = new Dictionary<Room, HashSet<(DayOfWeek, int)>>();

            foreach (var teacher in school.Teachers)
            {
                teacherAvailability[teacher] = new HashSet<(DayOfWeek, int)>();
            }

            foreach (var room in school.Rooms)
            {
                roomAvailability[room] = new HashSet<(DayOfWeek, int)>();
            }

            foreach (var subject in school.Subjects)
            {
                for (int i = 0; i < subject.WeeklyHours; i++)
                {
                    var availableTeachers = school.Teachers
                        .Where(t => t.Specializations.Contains(subject.Specialization))
                        .ToList();

                    if (!availableTeachers.Any())
                        throw new InvalidOperationException($"Нет доступных учителей для предмета {subject.Name}");

                    var availableRooms = school.Rooms
                        .Where(r => r.Type == RoomType.Standard)
                        .ToList();

                    if (!availableRooms.Any())
                        throw new InvalidOperationException($"Нет доступных кабинетов для предмета {subject.Name}");

                    bool scheduled = false;

                    foreach (var day in _daysOfWeek)
                    {
                        for (int lessonNumber = 1; lessonNumber <= 8; lessonNumber++) // Предположим 8 уроков в день
                        {
                            // Найти свободного учителя
                            var teacher = availableTeachers.FirstOrDefault(t =>
                                !teacherAvailability[t].Contains((day, lessonNumber)));

                            if (teacher == null)
                                continue;

                            // Найти свободный кабинет
                            var room = availableRooms.FirstOrDefault(r =>
                                !roomAvailability[r].Contains((day, lessonNumber)));

                            if (room == null)
                                continue;

                            // Назначить урок
                            var lessonSchedule = new LessonSchedule
                            {
                                Teacher = teacher,
                                Subject = subject,
                                Room = room,
                                School = school,
                                DayOfWeek = day,
                                LessonNumber = lessonNumber
                            };

                            generatedSchedules.Add(lessonSchedule);

                            // Обновить доступность
                            teacherAvailability[teacher].Add((day, lessonNumber));
                            roomAvailability[room].Add((day, lessonNumber));

                            scheduled = true;
                            break; // Переходим к следующему уроку
                        }

                        if (scheduled)
                            break;
                    }

                    if (!scheduled)
                    {
                        throw new InvalidOperationException(
                            $"Не удалось назначить урок для предмета {subject.Name}. Недостаточно ресурсов.");
                    }
                }
            }

            return generatedSchedules;
        }
    }
}