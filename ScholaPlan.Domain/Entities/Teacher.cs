using ScholaPlan.Domain.ValueObjects;
using ScholaPlan.Domain.Enums;

namespace ScholaPlan.Domain.Entities;

/// <summary>
/// Учитель в школе.
/// </summary>
public class Teacher
{
    public int Id { get; set; }

    public TeacherName Name { get; set; }

    public ICollection<TeacherSpecialization> Specializations { get; set; } = new List<TeacherSpecialization>();

    public int SchoolId { get; set; }

    public School School { get; set; }

    public ICollection<LessonSchedule> Lessons { get; set; } = new List<LessonSchedule>();

    public TeacherPreferences Preferences { get; set; }
}