using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;
using ScholaPlan.Domain.ValueObjects;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data;

/// <summary>
/// Класс для начального заполнения базы данных тестовыми данными
/// </summary>
public static class DbInitializer
{
    public static void Initialize(ScholaPlanDbContext context)
    {
        if (context.Schools.Any()) return;

        var school = new School
        {
            Name = "ScholaPlan Academy",
            Address = "123 Main Street"
        };

        // Добавление данных отдельно с явным указанием связи
        var maxLessonsPerDayConfigs = new List<MaxLessonsPerDayConfig>
        {
            new MaxLessonsPerDayConfig { School = school, ClassGrade = 7, MaxLessons = 6 },
            new MaxLessonsPerDayConfig { School = school, ClassGrade = 9, MaxLessons = 7 }
        };

        var subjects = new List<Subject>
        {
            new Subject { Name = "Mathematics", WeeklyHours = 5, DifficultyLevel = 8 },
            new Subject { Name = "Physics", WeeklyHours = 3, DifficultyLevel = 7 }
        };

        var rooms = new List<Room>
        {
            new Room { Number = "101", Type = RoomType.Standard, School = school },
            new Room { Number = "102", Type = RoomType.LanguageRoom, School = school }
        };

        var teachers = new List<Teacher>
        {
            new Teacher
            {
                Name = new TeacherName("John", "Doe"),
                Specializations = new List<TeacherSpecialization> { TeacherSpecialization.Mathematics },
                School = school
            },
            new Teacher
            {
                Name = new TeacherName("Jane", "Smith"),
                Specializations = new List<TeacherSpecialization> { TeacherSpecialization.Physics },
                School = school
            }
        };

        school.MaxLessonsPerDayConfigs = maxLessonsPerDayConfigs;
        school.Subjects = subjects;
        school.Rooms = rooms;
        school.Teachers = teachers;

        context.Schools.Add(school);
        context.SaveChanges();
    }

}