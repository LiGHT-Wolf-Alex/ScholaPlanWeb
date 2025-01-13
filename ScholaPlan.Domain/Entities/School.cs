using System.ComponentModel.DataAnnotations;
using ScholaPlan.Domain.Contract;

namespace ScholaPlan.Domain.Entities;

public class School
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Teacher> Teachers { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];
    public List<LessonSchedule> LessonSchedules { get; set; } = [];

    /// <summary>
    /// Максимальное количество уроков в день для каждого класса.
    /// </summary>
    public Dictionary<int, int> MaxLessonsPerDay { get; set; } = new Dictionary<int, int>();
}