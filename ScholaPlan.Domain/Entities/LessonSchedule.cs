using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities;

public class LessonSchedule
{
    public int Id { get; set; }

    [Required] public int SchoolId { get; set; }
    public School School { get; set; }

    [Required] public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    [Required] public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    [Required] public int RoomId { get; set; }
    public Room Room { get; set; }

    [Range(1, 12, ErrorMessage = "Класс должен быть от 1 до 12.")]
    public int ClassGrade { get; set; }

    [Range(1, 8, ErrorMessage = "Номер урока должен быть от 1 до 8.")]
    public int LessonNumber { get; set; }

    [Required] public DayOfWeek DayOfWeek { get; set; }
}