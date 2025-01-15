using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.API.DTOs;

/// <summary>
/// Модель запроса на генерацию расписания.
/// </summary>
public class GenerateScheduleRequest
{
    /// <summary>
    /// ID школы.
    /// </summary>
    [Required]
    public int SchoolId { get; set; }

    /// <summary>
    /// Предпочтения учителей.
    /// </summary>
    public Dictionary<int, TeacherPreferencesDto> TeacherPreferences { get; set; } =
        new Dictionary<int, TeacherPreferencesDto>();
}

/// <summary>
/// DTO модель для предпочтений учителя.
/// </summary>
public class TeacherPreferencesDto
{
    /// <summary>
    /// Доступные дни недели.
    /// </summary>
    public List<DayOfWeekDto> AvailableDays { get; set; } = new List<DayOfWeekDto>();

    /// <summary>
    /// Доступные номера уроков.
    /// </summary>
    public List<int> AvailableLessonNumbers { get; set; } = new List<int>();

    /// <summary>
    /// Предпочтительные кабинеты.
    /// </summary>
    public List<int> PreferredRoomIds { get; set; } = new List<int>();
}

/// <summary>
/// DTO модель для дня недели.
/// </summary>
public enum DayOfWeekDto
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday
}