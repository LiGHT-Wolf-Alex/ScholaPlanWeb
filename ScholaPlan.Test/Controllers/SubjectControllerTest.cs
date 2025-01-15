using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.API.Controllers;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Test.Controllers;

public class SubjectControllerTests
{
    private readonly SubjectController _controller;
    private readonly ScholaPlanDbContext _context;

    public SubjectControllerTests()
    {
        var options = new DbContextOptionsBuilder<ScholaPlanDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new ScholaPlanDbContext(options);
        _controller = new SubjectController(_context);
    }

    [Fact]
    public async Task Create_ValidSubject_ReturnsCreatedResult()
    {
        // Arrange
        var subject = new Subject { Name = "Mathematics", WeeklyHours = 5, DifficultyLevel = 8 };

        // Act
        var result = await _controller.Create(subject);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingSubject_ReturnsOkResult()
    {
        // Arrange
        var subject = new Subject { Name = "Physics", WeeklyHours = 3, DifficultyLevel = 7 };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(subject.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedSubject = Assert.IsType<Subject>(okResult.Value);
        Assert.Equal(subject.Id, returnedSubject.Id);
    }
}