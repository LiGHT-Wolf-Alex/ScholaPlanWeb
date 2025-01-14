using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities
{
    /// <summary>
    /// Сущность, представляющая школу.
    /// </summary>
    public class School
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Название школы не может превышать 200 символов.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Адрес школы не может превышать 500 символов.")]
        public string? Address { get; set; } // Добавил nullable

        /// <summary>
        /// Список ограничений по количеству уроков для каждого класса
        /// </summary>
        public ICollection<MaxLessonsPerDayConfig> MaxLessonsPerDayConfigs { get; set; } =
            new List<MaxLessonsPerDayConfig>();

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<LessonSchedule> LessonSchedules { get; set; } = new List<LessonSchedule>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}