using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.API.Controllers;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;
using ScholaPlan.Infrastructure.Data.Context;

public class RoomControllerTests
{
    private readonly RoomController _controller;
    private readonly ScholaPlanDbContext _context;

    public RoomControllerTests()
    {
        var options = new DbContextOptionsBuilder<ScholaPlanDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new ScholaPlanDbContext(options);
        _controller = new RoomController(_context);
    }

    [Fact]
    public async Task Create_ValidRoom_ReturnsCreatedResult()
    {
        // Arrange
        var school = new School { Name = "Test School" };
        _context.Schools.Add(school);
        await _context.SaveChangesAsync();

        var room = new Room { Number = "101", Type = RoomType.Standard, SchoolId = school.Id };

        // Act
        var result = await _controller.Create(room);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingRoom_ReturnsOkResult()
    {
        // Arrange
        var school = new School { Name = "Test School" };
        _context.Schools.Add(school);
        var room = new Room { Number = "102", Type = RoomType.LanguageRoom, School = school };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(room.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoom = Assert.IsType<Room>(okResult.Value);
        Assert.Equal(room.Id, returnedRoom.Id);
    }
}