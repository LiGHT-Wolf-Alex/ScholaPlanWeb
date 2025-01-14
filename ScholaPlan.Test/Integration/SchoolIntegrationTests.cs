using Xunit;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.API.Controllers;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScholaPlan.API.DTOs;
using ScholaPlan.Application.Interfaces.IRepositories;

namespace ScholaPlan.Tests.Integration
{
    public class SchoolIntegrationTests
    {
        private readonly ScholaPlanDbContext _context;
        private readonly ScheduleController _scheduleController;

        public SchoolIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ScholaPlanDbContext>()
                .UseInMemoryDatabase(databaseName: "IntegrationTestDb")
                .Options;

            _context = new ScholaPlanDbContext(options);
            var mockSchoolRepo = new Mock<ISchoolRepository>();
            var mockScheduleService = new Mock<IScheduleService>();

            _scheduleController = new ScheduleController(mockScheduleService.Object, mockSchoolRepo.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFoundWhenSchoolDoesNotExist()
        {
            var request = new GenerateScheduleRequest { SchoolId = 999 }; 
            var result = await _scheduleController.GenerateSchedule(request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ShouldGenerateScheduleForSchool()
        {
            var school = new School { Name = "Test School", Address = "123 Test Street" };
            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            var mockSchoolRepo = new Mock<ISchoolRepository>();
            mockSchoolRepo.Setup(r => r.GetByIdAsync(school.Id)).ReturnsAsync(school);

            var mockScheduleService = new Mock<IScheduleService>();
            mockScheduleService.Setup(s => s.GenerateSchedule(school)).Returns(new List<LessonSchedule>());

            var scheduleController = new ScheduleController(mockScheduleService.Object, mockSchoolRepo.Object);
            var request = new GenerateScheduleRequest { SchoolId = school.Id };
            var result = await scheduleController.GenerateSchedule(request);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}