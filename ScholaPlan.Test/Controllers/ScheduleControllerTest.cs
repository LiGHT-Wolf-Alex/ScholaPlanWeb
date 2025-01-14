using Microsoft.AspNetCore.Mvc;
using Moq;
using ScholaPlan.API.Controllers;
using ScholaPlan.API.DTOs;
using Xunit;
using ScholaPlan.Application.Services;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Tests.Application.Services;

public class ScheduleControllerTests
{
    private readonly ScheduleController _scheduleController;
    private readonly Mock<IScheduleService> _mockScheduleService;
    private readonly Mock<ISchoolRepository> _mockSchoolRepository;

    public ScheduleControllerTests()
    {
        _mockScheduleService = new Mock<IScheduleService>();
        _mockSchoolRepository = new Mock<ISchoolRepository>();
        _scheduleController = new ScheduleController(_mockScheduleService.Object, _mockSchoolRepository.Object);
    }

    [Fact]
    public async Task GenerateSchedule_ReturnsNotFound_WhenSchoolDoesNotExist()
    {
        // Arrange
        _mockSchoolRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((School?)null);
        var request = new GenerateScheduleRequest { SchoolId = 1 };

        // Act
        var result = await _scheduleController.GenerateSchedule(request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ShouldGenerateScheduleForSchool()
    {
        var school = new School { Name = "Test School", Address = "123 Test Street" };

        var mockSchoolRepo = new Mock<ISchoolRepository>();
        mockSchoolRepo.Setup(r => r.GetByIdAsync(school.Id)).ReturnsAsync(school);

        var mockScheduleService = new Mock<IScheduleService>();
        mockScheduleService.Setup(s => s.GenerateSchedule(school)).Returns(new List<LessonSchedule>());

        var scheduleController = new ScheduleController(mockScheduleService.Object, mockSchoolRepo.Object);
        var request = new GenerateScheduleRequest { SchoolId = school.Id };
    
        var result = await scheduleController.GenerateSchedule(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Расписание успешно сгенерировано.", okResult.Value);
    }



    [Fact]
    public async Task GenerateSchedule_ReturnsServerError_OnException()
    {
        // Arrange
        var school = new School { Name = "Test School" };
        _mockSchoolRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(school);
        _mockScheduleService.Setup(service => service.GenerateSchedule(school))
            .Throws(new Exception("Тестовое исключение"));

        var request = new GenerateScheduleRequest { SchoolId = 1 };

        // Act
        var result = await _scheduleController.GenerateSchedule(request);

        // Assert
        var serverErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverErrorResult.StatusCode);
        Assert.Contains("Ошибка сервера", serverErrorResult.Value.ToString());
    }
}