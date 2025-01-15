using System.ComponentModel.DataAnnotations;

namespace ScholaPlan.API.DTOs;

/// <summary>
/// Модель для назначения или удаления роли пользователю.
/// </summary>
public class AssignRoleModel
{
    /// <summary>
    /// ID пользователя.
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    [Required]
    public string Role { get; set; }
}