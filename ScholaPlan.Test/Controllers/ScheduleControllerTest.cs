/*
using JetBrains.Annotations;
using ScholaPlan.API.Controllers;
using ScholaPlan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;

namespace ScholaPlan.Test.Controllers;

[TestSubject(typeof(ScheduleController))]
public class ScheduleControllerTests
{
    private readonly ScheduleController _controller;
    private readonly Mock<IScheduleGenerator> _scheduleGeneratorMock;

    public ScheduleControllerTests()
    {
        _scheduleGeneratorMock = new Mock<IScheduleGenerator>();
        _controller = new ScheduleController(_scheduleGeneratorMock.Object);
    }

    [Fact]
    public async Task GenerateSchedule_ReturnsOk_WhenScheduleIsGenerated()
    {
        // Arrange
        var request = new GenerateScheduleRequest();
        _scheduleGeneratorMock
            .Setup(sg => sg.GenerateScheduleAsync(request))
            .ReturnsAsync(new GenerateScheduleResponse());

        // Act
        var result = await _controller.GenerateSchedule(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GenerateSchedule_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Invalid data");

        // Act
        var result = await _controller.GenerateSchedule(new GenerateScheduleRequest());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}*/

