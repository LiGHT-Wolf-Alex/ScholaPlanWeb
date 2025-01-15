using Microsoft.EntityFrameworkCore;
using ScholaPlan.Infrastructure.Data;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Test.Data;

public class DbInitializerTests
{
    private readonly ScholaPlanDbContext _context;

    public DbInitializerTests()
    {
        var options = new DbContextOptionsBuilder<ScholaPlanDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbInitializer")
            .Options;

        _context = new ScholaPlanDbContext(options);
    }

    [Fact]
    public void Initialize_ShouldPopulateDatabase_WhenCalled()
    {
        // Act
        DbInitializer.Initialize(_context);

        // Assert
        Assert.True(_context.Schools.Any(), "Schools should be populated");
        Assert.True(_context.Subjects.Any(), "Subjects should be populated");
        Assert.True(_context.Rooms.Any(), "Rooms should be populated");
        Assert.True(_context.Teachers.Any(), "Teachers should be populated");
        Assert.True(_context.MaxLessonsPerDayConfigs.Any(), "MaxLessonsPerDayConfigs should be populated");
    }

    [Fact]
    public void Initialize_ShouldNotDuplicateData_WhenCalledMultipleTimes()
    {
        // Act
        DbInitializer.Initialize(_context);
        DbInitializer.Initialize(_context);

        // Assert
        Assert.Equal(1, _context.Schools.Count());
        Assert.Equal(2, _context.Subjects.Count());
        Assert.Equal(2, _context.Rooms.Count());
        Assert.Equal(2, _context.Teachers.Count());
        Assert.Equal(2, _context.MaxLessonsPerDayConfigs.Count());
    }
}