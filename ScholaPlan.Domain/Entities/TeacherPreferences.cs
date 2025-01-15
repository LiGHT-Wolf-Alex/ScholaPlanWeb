using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.Domain.Entities;

/// <summary>
/// Модель для хранения предпочтений учителя по доступности.
/// </summary>
public class TeacherPreferences
{
    [Key] public int TeacherId { get; set; }

    /// <summary>
    /// Доступные дни недели для учителя.
    /// </summary>
    public List<DayOfWeek> AvailableDays { get; set; } = new List<DayOfWeek>();

    /// <summary>
    /// Доступные номера уроков для учителя.
    /// </summary>
    public List<int> AvailableLessonNumbers { get; set; } = new List<int>();

    /// <summary>
    /// Предпочтительные кабинеты для учителя.
    /// </summary>
    public List<int> PreferredRoomIds { get; set; } = new List<int>();

    // Навигационное свойство
    public Teacher Teacher { get; set; }
}