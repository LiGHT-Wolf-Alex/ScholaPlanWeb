using Xunit;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Application.Services;
using System.Linq;
using ScholaPlan.Domain.Enums;
using ScholaPlan.Domain.ValueObjects;

namespace ScholaPlan.Tests.Application.Services
{
    public class ScheduleGeneratorTests
    {
        private readonly IScheduleGenerator _scheduleGenerator;

        public ScheduleGeneratorTests()
        {
            _scheduleGenerator = new ScheduleGenerator();
        }

        [Fact]
        public void GenerateSchedule_ShouldReturnSchedules_WhenValidSchoolProvided()
        {
            // Arrange
            var school = new School
            {
                Subjects = new List<Subject>
                {
                    new Subject { Name = "Math", WeeklyHours = 5, Specialization = TeacherSpecialization.Mathematics }
                },
                Teachers = new List<Teacher>
                {
                    new Teacher
                    {
                        Name = new TeacherName("John", "Doe"),
                        Specializations = new List<TeacherSpecialization> { TeacherSpecialization.Mathematics }
                    }
                },
                Rooms = new List<Room>
                {
                    new Room { Number = "101", Type = RoomType.Standard }
                }
            };

            // Act
            var schedule = _scheduleGenerator.GenerateSchedule(school).ToList();

// Assert
            Assert.NotEmpty(schedule);
            Assert.Equal(school.Subjects.Sum(s => s.WeeklyHours), schedule.Count);
        }

        [Fact]
        public void GenerateSchedule_ShouldThrowException_WhenNoTeachersAvailable()
        {
            // Arrange
            var school = new School
            {
                Subjects = new List<Subject>
                {
                    new Subject { Name = "Math", WeeklyHours = 5, Specialization = TeacherSpecialization.Mathematics }
                },
                Teachers = new List<Teacher>(), // Нет учителей
                Rooms = new List<Room>
                {
                    new Room { Number = "101", Type = RoomType.Standard }
                }
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _scheduleGenerator.GenerateSchedule(school));
            Assert.Contains("Нет доступных учителей", exception.Message);
        }

        [Fact]
        public void GenerateSchedule_ShouldThrowException_WhenNoRoomsAvailable()
        {
            // Arrange
            var school = new School
            {
                Subjects = new List<Subject>
                {
                    new Subject { Name = "Physics", WeeklyHours = 3, Specialization = TeacherSpecialization.Physics }
                },
                Teachers = new List<Teacher>
                {
                    new Teacher
                    {
                        Name = new TeacherName("Jane", "Smith"),
                        Specializations = new List<TeacherSpecialization> { TeacherSpecialization.Physics }
                    }
                },
                Rooms = new List<Room>() // Нет кабинетов
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _scheduleGenerator.GenerateSchedule(school));
            Assert.Contains("Нет доступных кабинетов", exception.Message);
        }
    }
}