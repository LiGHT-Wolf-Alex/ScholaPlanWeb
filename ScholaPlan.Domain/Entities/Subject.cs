using ScholaPlan.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Название предмета не может превышать 100 символов.")]
        public string Name { get; set; }
        
        public int MaxWeaklyLesson { get; set; }
        

        [Range(1, 12, ErrorMessage = "Уровень сложности должен быть от 1 до 12.")]
        public int DifficultyLevel { get; set; }

        [Range(1, 5, ErrorMessage = "Количество часов в неделю должно быть от 1 до 5.")]
        public int WeeklyHours { get; set; }

        [Required] public TeacherSpecialization Specialization { get; set; }
    }
}