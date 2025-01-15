using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.API.DTOs;

/// <summary>
/// Модель для регистрации нового пользователя.
/// </summary>
public class RegisterModel
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [MinLength(4, ErrorMessage = "Имя пользователя должно содержать минимум 4 символа.")]
    public string Username { get; set; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Неверный формат электронной почты.")]
    public string Email { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов.")]
    public string Password { get; set; }

    /// <summary>
    /// Подтверждение пароля.
    /// </summary>
    [Required]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; }
}