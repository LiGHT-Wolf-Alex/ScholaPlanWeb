﻿using ScholaPlan.Application.Interfaces;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace ScholaPlan.Application.Services;

public class ScheduleService(
    IScheduleGenerator scheduleGenerator,
    IUnitOfWork unitOfWork,
    ILogger<ScheduleService> logger)
    : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<LessonSchedule>> GenerateScheduleAsync(School school,
        Dictionary<int, TeacherPreferences> teacherPreferences)
    {
        if (!school.Subjects.Any() || !school.Teachers.Any())
        {
            logger.LogWarning("Недостаточно данных для генерации расписания.");
            throw new InvalidOperationException("Недостаточно данных для генерации расписания");
        }

        try
        {
            var schedules = (await scheduleGenerator.GenerateScheduleAsync(school, teacherPreferences)).ToList();
            logger.LogInformation($"Сгенерировано {schedules.Count} занятий для школы ID {school.Id}.");
            return schedules;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при генерации расписания.");
            throw;
        }
    }
}