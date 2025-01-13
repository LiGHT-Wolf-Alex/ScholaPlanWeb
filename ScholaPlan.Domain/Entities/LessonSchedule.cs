namespace ScholaPlan.Domain.Entities;

public class LessonSchedule
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public School School { get; set; }

    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; }

    public int ClassGrade { get; set; } // Класс (например, 7, 8, 9)
    public int LessonNumber { get; set; } // Номер урока в течение дня (1-7)
    public DayOfWeek DayOfWeek { get; set; }
}