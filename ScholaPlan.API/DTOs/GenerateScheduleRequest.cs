using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.API.DTOs
{
    /// <summary>
    /// Запрос для генерации расписания.
    /// </summary>
    public class GenerateScheduleRequest
    {
        /// <summary>
        /// Идентификатор школы для генерации расписания.
        /// </summary>
        [Required]
        public int SchoolId { get; set; }

        /// <summary>
        /// Учебный год для генерации расписания.
        /// </summary>
        [Required]
        [StringLength(9, MinimumLength = 4, ErrorMessage = "Учебный год должен быть в формате YYYY-YYYY.")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Учебный год должен быть в формате YYYY-YYYY.")]
        public string AcademicYear { get; set; }
    }
}