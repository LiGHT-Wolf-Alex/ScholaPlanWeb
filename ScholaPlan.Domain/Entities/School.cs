using System.ComponentModel.DataAnnotations;
using ScholaPlan.Domain.Contract;

namespace ScholaPlan.Domain.Entities;

/// <summary>
/// Сущность, представляющая школу.
/// </summary>
public class School
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    /// <summary>
    /// Список ограничений по количеству уроков для каждого класса
    /// </summary>
    public ICollection<MaxLessonsPerDayConfig> MaxLessonsPerDayConfigs { get; set; } =
        new List<MaxLessonsPerDayConfig>();

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public ICollection<LessonSchedule> LessonSchedules { get; set; } = new List<LessonSchedule>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}