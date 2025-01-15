using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ScholaPlan.Application.Services;

public class ScheduleGenerator(ILogger<ScheduleGenerator> logger) : IScheduleGenerator
{
    private readonly IEnumerable<DayOfWeek> _daysOfWeek = new[]
    {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday
    };

    public async Task<IEnumerable<LessonSchedule>> GenerateScheduleAsync(School school,
        Dictionary<int, TeacherPreferences> teacherPreferences)
    {
        return await Task.Run(() => GenerateSchedule(school, teacherPreferences));
    }

    private IEnumerable<LessonSchedule> GenerateSchedule(School school,
        Dictionary<int, TeacherPreferences> teacherPreferences)
    {
        var generatedSchedules = new ConcurrentBag<LessonSchedule>();

        var teacherAvailability = new ConcurrentDictionary<Teacher, HashSet<(DayOfWeek, int)>>();
        var roomAvailability = new ConcurrentDictionary<Room, HashSet<(DayOfWeek, int)>>();

        foreach (var teacher in school.Teachers)
            teacherAvailability[teacher] = new HashSet<(DayOfWeek, int)>();

        foreach (var room in school.Rooms)
            roomAvailability[room] = new HashSet<(DayOfWeek, int)>();

        var lessonsCountByClassDay = new ConcurrentDictionary<(int, DayOfWeek), int>();

        var orderedClassConfigs =
            school.MaxLessonsPerDayConfigs.OrderByDescending(c => c.MaxLessonsPerDay).ToList();

        foreach (var classConfig in orderedClassConfigs)
        {
            int classGrade = classConfig.ClassGrade;
            int maxLessonsPerDay = classConfig.MaxLessonsPerDay;

            var orderedSubjects = school.Subjects.OrderByDescending(s => s.WeeklyHours).ToList();

            foreach (var subject in orderedSubjects)
            {
                for (int i = 0; i < subject.WeeklyHours; i++)
                {
                    var availableTeachers = school.Teachers
                        .Where(t => t.Specializations.Contains(subject.Specialization))
                        .OrderBy(t => teacherAvailability[t].Count)
                        .ToList();

                    if (!availableTeachers.Any())
                    {
                        logger.LogWarning(
                            $"Нет доступных учителей для предмета {subject.Name} в классе {classGrade}.");
                        throw new InvalidOperationException($"Нет доступных учителей для предмета {subject.Name}");
                    }

                    var availableRooms = school.Rooms
                        .Where(r => r.Type == RoomType.Standard)
                        .OrderBy(r => roomAvailability[r].Count)
                        .ToList();

                    if (!availableRooms.Any())
                    {
                        logger.LogWarning(
                            $"Нет доступных кабинетов для предмета {subject.Name} в классе {classGrade}.");
                        throw new InvalidOperationException($"Нет доступных кабинетов для предмета {subject.Name}");
                    }

                    bool scheduled = false;

                    foreach (var day in _daysOfWeek)
                    {
                        for (int lessonNumber = 1; lessonNumber <= 8; lessonNumber++)
                        {
                            var key = (classGrade, day);
                            lessonsCountByClassDay.AddOrUpdate(key, 0, (k, v) => v);

                            if (lessonsCountByClassDay[key] >= maxLessonsPerDay)
                                continue;

                            var suitableTeachers = availableTeachers.Where(t =>
                            {
                                // Проверка доступности по предпочтениям
                                if (teacherPreferences.ContainsKey(t.Id))
                                {
                                    var prefs = teacherPreferences[t.Id];
                                    if (!prefs.AvailableDays.Contains(day) ||
                                        !prefs.AvailableLessonNumbers.Contains(lessonNumber))
                                        return false;
                                }

                                return !teacherAvailability[t].Contains((day, lessonNumber));
                            }).ToList();

                            if (!suitableTeachers.Any())
                                continue;

                            // Попытка найти учителя, у которого уже есть занятия в этом кабинете в тот же день
                            Teacher selectedTeacher = null;
                            Room selectedRoom = null;

                            foreach (var teacher in suitableTeachers)
                            {
                                if (teacherPreferences.ContainsKey(teacher.Id))
                                {
                                    var prefs = teacherPreferences[teacher.Id];
                                    foreach (var preferredRoomId in prefs.PreferredRoomIds)
                                    {
                                        var room = school.Rooms.FirstOrDefault(r => r.Id == preferredRoomId);
                                        if (room != null && !roomAvailability[room].Contains((day, lessonNumber)))
                                        {
                                            selectedTeacher = teacher;
                                            selectedRoom = room;
                                            break;
                                        }
                                    }

                                    if (selectedTeacher != null)
                                        break;
                                }
                            }

                            // Если не удалось найти в предпочтительных кабинетах, выбрать любой доступный
                            if (selectedTeacher == null)
                            {
                                selectedTeacher = suitableTeachers.First();
                                selectedRoom = availableRooms.FirstOrDefault(r =>
                                    !roomAvailability[r].Contains((day, lessonNumber)));
                            }

                            if (selectedTeacher != null && selectedRoom != null)
                            {
                                var lessonSchedule = new LessonSchedule
                                {
                                    School = school,
                                    SchoolId = school.Id,

                                    Teacher = selectedTeacher,
                                    TeacherId = selectedTeacher.Id,

                                    Subject = subject,
                                    SubjectId = subject.Id,

                                    Room = selectedRoom,
                                    RoomId = selectedRoom.Id,

                                    ClassGrade = classGrade,
                                    DayOfWeek = day,
                                    LessonNumber = lessonNumber
                                };

                                generatedSchedules.Add(lessonSchedule);

                                lock (teacherAvailability[selectedTeacher])
                                {
                                    teacherAvailability[selectedTeacher].Add((day, lessonNumber));
                                }

                                lock (roomAvailability[selectedRoom])
                                {
                                    roomAvailability[selectedRoom].Add((day, lessonNumber));
                                }

                                lessonsCountByClassDay.AddOrUpdate(key, 1, (k, v) => v + 1);

                                scheduled = true;
                                break;
                            }
                        }

                        if (scheduled)
                            break;
                    }

                    if (!scheduled)
                    {
                        logger.LogWarning(
                            $"Не удалось назначить урок (Class={classGrade}, Subject={subject.Name}). Недостаточно ресурсов или превышен лимит уроков.");
                        throw new InvalidOperationException(
                            $"Не удалось назначить урок (Class={classGrade}, Subject={subject.Name}). " +
                            "Недостаточно ресурсов или превышен лимит уроков.");
                    }
                }
            }
        }

        logger.LogInformation($"Генерация расписания завершена. Всего занятий: {generatedSchedules.Count}.");
        return generatedSchedules;
    }
}