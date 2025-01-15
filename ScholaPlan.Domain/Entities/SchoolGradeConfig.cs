using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities;

/// <summary>
/// Конфигурация максимального количества уроков для школы
/// </summary>
public class SchoolGradeConfig
{
    public int Id { get; set; }

    /// <summary>
    /// Ссылка на школу
    /// </summary>
    [Required]
    public int SchoolId { get; set; }

    public School School { get; set; }

    /// <summary>
    /// Класс, для которого указано количество уроков
    /// </summary>
    [Range(1, 12, ErrorMessage = "Класс должен быть от 1 до 12.")]
    public int ClassGrade { get; set; }

    /// <summary>
    /// Максимальное количество уроков в день
    /// </summary>
    [Range(1, 10, ErrorMessage = "Максимальное количество уроков должно быть от 1 до 10.")]
    public int MaxLessonsPerDay { get; set; }
}