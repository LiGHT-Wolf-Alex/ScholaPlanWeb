using ScholaPlan.Domain.Contract;
using ScholaPlan.Domain.Enums;
using ScholaPlan.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required] public TeacherName Name { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Учитель должен иметь хотя бы одну специализацию.")]
        public List<TeacherSpecialization> Specializations { get; set; } = new List<TeacherSpecialization>();

        [Required] public int SchoolId { get; set; }

        public School School { get; set; }

        public ICollection<LessonSchedule> Lessons { get; set; } = new List<LessonSchedule>();
    }
}