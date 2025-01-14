using ScholaPlan.Application.Interfaces;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace ScholaPlan.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleGenerator _scheduleGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IScheduleGenerator scheduleGenerator, IUnitOfWork unitOfWork,
            ILogger<ScheduleService> logger)
        {
            _scheduleGenerator = scheduleGenerator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<LessonSchedule>> GenerateScheduleAsync(School school,
            Dictionary<int, TeacherPreferences> teacherPreferences)
        {
            if (!school.Subjects.Any() || !school.Teachers.Any())
            {
                _logger.LogWarning("Недостаточно данных для генерации расписания.");
                throw new InvalidOperationException("Недостаточно данных для генерации расписания");
            }

            try
            {
                var schedules = (await _scheduleGenerator.GenerateScheduleAsync(school, teacherPreferences)).ToList();
                _logger.LogInformation($"Сгенерировано {schedules.Count} занятий для школы ID {school.Id}.");
                return schedules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации расписания.");
                throw;
            }
        }
    }
}