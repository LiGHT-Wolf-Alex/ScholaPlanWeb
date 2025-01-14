namespace ScholaPlan.API.DTOs
{
    /// <summary>
    /// DTO модель для представления пользователя.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID пользователя.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Электронная почта пользователя.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Роли пользователя.
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
}