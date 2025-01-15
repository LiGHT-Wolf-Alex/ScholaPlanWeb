using ScholaPlan.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities;

public class Room
{
    public int Id { get; set; }

    [Required]
    [StringLength(10, ErrorMessage = "Номер кабинета не может превышать 10 символов.")]
    public string Number { get; set; }

    [Required] public RoomType Type { get; set; }

    [Required] public int SchoolId { get; set; }
    public School School { get; set; }

    public ICollection<LessonSchedule> Lessons { get; set; } = new List<LessonSchedule>();
}