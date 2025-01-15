namespace ScholaPlan.API.DTOs;

/// <summary>
/// Модель ответа на генерацию расписания.
/// </summary>
public class GenerateScheduleResponse
{
    /// <summary>
    /// Успешность операции.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об операции.
    /// </summary>
    public string Message { get; set; }
}