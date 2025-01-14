namespace ScholaPlan.API.DTOs;

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