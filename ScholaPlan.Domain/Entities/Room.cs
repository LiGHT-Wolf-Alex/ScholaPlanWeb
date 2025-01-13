using ScholaPlan.Domain.Enums;

namespace ScholaPlan.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public string Number { get; set; }
    public RoomType Type { get; set; }

    public int SchoolId { get; set; }
    public School School { get; set; }

    public ICollection<LessonSchedule> Lessons { get; set; } = new List<LessonSchedule>();
}