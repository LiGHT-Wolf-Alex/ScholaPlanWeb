namespace ScholaPlan.API.DTOs;

/// <summary>
/// DTO модель для представления расписания занятия.
/// </summary>
public class LessonScheduleDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolName { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; }
    public int ClassGrade { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int LessonNumber { get; set; }
}