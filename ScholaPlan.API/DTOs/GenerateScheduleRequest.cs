namespace ScholaPlan.API.DTOs;

/// <summary>
/// Запрос для генерации расписания.
/// </summary>
public class GenerateScheduleRequest
{
    /// <summary>
    /// Идентификатор школы для генерации расписания.
    /// </summary>
    public int SchoolId { get; set; }

    /// <summary>
    /// Учебный год для генерации расписания.
    /// </summary>
    public string AcademicYear { get; set; }
}

/// <summary>
/// Ответ с результатом генерации расписания.
/// </summary>
public class GenerateScheduleResponse
{
    /// <summary>
    /// Успешно ли сгенерировано расписание.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке или результате генерации.
    /// </summary>
    public string Message { get; set; }
}