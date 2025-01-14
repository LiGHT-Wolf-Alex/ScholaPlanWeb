using ScholaPlan.Domain.Contract;
using ScholaPlan.Domain.Enums;
using ScholaPlan.Domain.ValueObjects;

namespace ScholaPlan.Domain.Entities;

public class Teacher
{
    public int Id { get; set; }
    public TeacherName Name { get; set; }
    public List<TeacherSpecialization> Specializations { get; set; } = new List<TeacherSpecialization>();
    public int SchoolId { get; set; }

    public School School { get; set; }

    public ICollection<LessonSchedule> Lessons { get; set; } = new List<LessonSchedule>();
}