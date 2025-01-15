
using Moq;
using ScholaPlan.API.Controllers;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.Test.Controllers;
using ScheduleController = ScholaPlan.Test.Controllers.ScheduleController;

namespace ScholaPlan.Tests;

public class ScheduleControllerTests
{
    private readonly Mock<IScheduleService> _scheduleServiceMock;
    private readonly ScheduleController _scheduleController;

    public ScheduleControllerTests()
    {
        _scheduleServiceMock = new Mock<IScheduleService>();
        _scheduleController = new ScheduleController(_scheduleServiceMock.Object);
    }

    [Fact]
    public async Task GenerateSchedule_ValidRequest_ReturnsOkWithSchedules()
    {
        // Arrange
        var request = new ScheduleRequest
        {
            School = new School { Id = 1, Name = "Test School", Address = "123 Test St" },
            TeacherPreferences = new Dictionary<int, TeacherPreferences>
            {
                { 1, new TeacherPreferences { TeacherId = 1, AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday }, AvailableLessonNumbers = new List<int> { 1, 2 }, PreferredRoomIds = new List<int> { 101 } } }
            }
        };

        var expectedSchedules = new List<LessonSchedule>
        {
            new LessonSchedule { Id = 1, Subject = "Mathematics", TeacherId = 1, RoomId = 101, Day = DayOfWeek.Monday, LessonNumber = 1 }
        };

        _scheduleServiceMock.Setup(ss => ss.GenerateScheduleAsync(request.School, request.TeacherPreferences))
            .ReturnsAsync(expectedSchedules);

        // Act
        var result = await _scheduleController.GenerateSchedule(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expectedSchedules, okResult.Value);
    }

    [Fact]
    public async Task GenerateSchedule_ServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var request = new ScheduleRequest
        {
            School = new School { Id = 1, Name = "Test School", Address = "123 Test St" },
            TeacherPreferences = new Dictionary<int, TeacherPreferences>()
        };

        _scheduleServiceMock.Setup(ss => ss.GenerateScheduleAsync(request.School, request.TeacherPreferences))
            .ThrowsAsync(new System.Exception("Failed to generate schedule"));

        // Act
        var result = await _scheduleController.GenerateSchedule(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Failed to generate schedule", badRequestResult.Value);
    }
}