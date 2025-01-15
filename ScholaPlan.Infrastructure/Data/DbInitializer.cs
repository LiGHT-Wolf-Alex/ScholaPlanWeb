using Microsoft.AspNetCore.Identity;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.Extensions.DependencyInjection;
using ScholaPlan.Domain.Enums;
using ScholaPlan.Domain.ValueObjects;

namespace ScholaPlan.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(ScholaPlanDbContext context, IServiceProvider serviceProvider)
    {
        // Применение миграций и создание базы данных
        context.Database.EnsureCreated();

        // Создание ролей
        if (!context.Roles.Any())
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roles = { "Admin", "Teacher", "User" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    var newRole = new ApplicationRole { Name = role, NormalizedName = role.ToUpper() };
                    roleManager.CreateAsync(newRole).Wait();
                }
            }
        }

        // Создание пользователей
        if (!context.Users.Any())
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Создание администратора
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@scholaplan.com",
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(adminUser, "Admin@123").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            }

            // Создание учителя
            var teacherUser = new ApplicationUser
            {
                UserName = "teacher1",
                Email = "teacher1@scholaplan.com",
                EmailConfirmed = true
            };

            var teacherResult = userManager.CreateAsync(teacherUser, "Teacher@123").Result;
            if (teacherResult.Succeeded)
            {
                userManager.AddToRoleAsync(teacherUser, "Teacher").Wait();
            }

            // Создание обычного пользователя
            var normalUser = new ApplicationUser
            {
                UserName = "user1",
                Email = "user1@scholaplan.com",
                EmailConfirmed = true
            };

            var userResult = userManager.CreateAsync(normalUser, "User@123").Result;
            if (userResult.Succeeded)
            {
                userManager.AddToRoleAsync(normalUser, "User").Wait();
            }
        }

        // Добавление остальных данных
        if (context.Schools.Any()) return;

        var school = new School
        {
            Name = "ScholaPlan Academy",
            Address = "123 Main Street"
        };

        // Добавление данных с явным указанием связей
        var maxLessonsPerDayConfigs = new List<SchoolGradeConfig>
        {
            new SchoolGradeConfig { School = school, ClassGrade = 7, MaxLessonsPerDay = 6 },
            new SchoolGradeConfig { School = school, ClassGrade = 9, MaxLessonsPerDay = 7 }
        };

        var subjects = new List<Subject>
        {
            new Subject
            {
                Name = "Mathematics", WeeklyHours = 5, DifficultyLevel = 8,
                Specialization = TeacherSpecialization.Mathematics
            },
            new Subject
            {
                Name = "Physics", WeeklyHours = 3, DifficultyLevel = 7,
                Specialization = TeacherSpecialization.Physics
            }
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

        // Добавление предпочтений учителей
        var teacherPreferences = new List<TeacherPreferences>
        {
            new TeacherPreferences
            {
                TeacherId = teachers[0].Id, // Предполагается, что ID установлен после сохранения
                AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                AvailableLessonNumbers = new List<int> { 1, 2, 3, 4 },
                PreferredRoomIds = new List<int> { rooms[0].Id } // Предпочтительный кабинет 101
            },
            new TeacherPreferences
            {
                TeacherId = teachers[1].Id, // Предполагается, что ID установлен после сохранения
                AvailableDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday },
                AvailableLessonNumbers = new List<int> { 5, 6, 7, 8 },
                PreferredRoomIds = new List<int> { rooms[1].Id } // Предпочтительный кабинет 102
            }
        };

        context.TeacherPreferences.AddRange(teacherPreferences);
        context.SaveChanges();
    }
}