using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.API.DTOs
{
    /// <summary>
    /// Модель для аутентификации пользователя.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}