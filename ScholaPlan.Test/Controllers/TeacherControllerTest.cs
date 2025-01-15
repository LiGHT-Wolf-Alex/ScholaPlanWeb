using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.Controllers;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.ValueObjects;

namespace ScholaPlan.Tests.Controllers;

public class TeacherControllerTests
{
    private readonly TeacherController _controller;
    private readonly ScholaPlanDbContext _context;

    public TeacherControllerTests()
    {
        var options = new DbContextOptionsBuilder<ScholaPlanDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new ScholaPlanDbContext(options);

        _controller = new TeacherController(_context);
    }

    [Fact]
    public async Task Create_ValidTeacher_ReturnsCreatedResult()
    {
        // Arrange
        var school = new School { Name = "Test School" };
        _context.Schools.Add(school);
        await _context.SaveChangesAsync();

        var teacher = new Teacher
        {
            Name = new TeacherName("John", "Doe"),
            SchoolId = school.Id
        };

        // Act
        var result = await _controller.Create(teacher);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingTeacher_ReturnsOkResult()
    {
        // Arrange
        var school = new School { Name = "Test School" };
        _context.Schools.Add(school);
        var teacher = new Teacher
        {
            Name = new TeacherName("Jane", "Doe"),
            School = school
        };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(teacher.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTeacher = Assert.IsType<Teacher>(okResult.Value);
        Assert.Equal(teacher.Id, returnedTeacher.Id);
    }
}