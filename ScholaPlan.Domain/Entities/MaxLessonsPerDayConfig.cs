namespace ScholaPlan.Domain.Entities;

/// <summary>
/// Конфигурация максимального количества уроков для школы
/// </summary>
public class MaxLessonsPerDayConfig
{
    public int Id { get; set; }

    /// <summary>
    /// Ссылка на школу
    /// </summary>
    public int SchoolId { get; set; }

    public School School { get; set; }

    /// <summary>
    /// Класс, для которого указано количество уроков
    /// </summary>
    public int ClassGrade { get; set; }

    /// <summary>
    /// Максимальное количество уроков в день
    /// </summary>
    public int MaxLessons { get; set; }
}