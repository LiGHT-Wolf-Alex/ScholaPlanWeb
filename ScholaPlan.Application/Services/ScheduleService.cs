using ScholaPlan.Application.Interfaces;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Services;

/// <summary>
/// Сервис для генерации и управления расписанием.
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IScheduleGenerator _scheduleGenerator;

    public ScheduleService(IScheduleGenerator scheduleGenerator)
    {
        _scheduleGenerator = scheduleGenerator;
    }

    public List<LessonSchedule> GenerateSchedule(School school)
    {
        if (!school.Subjects.Any() || !school.Teachers.Any())
        {
            throw new InvalidOperationException("Недостаточно данных для генерации расписания");
        }

        return _scheduleGenerator.GenerateSchedule(school).ToList();
    }
}