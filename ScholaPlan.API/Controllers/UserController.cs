using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Только администраторы могут управлять пользователями
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<UserController> _logger;

    public UserController(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Получение списка всех пользователей.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.UserName,
            Email = u.Email
        }).ToList();

        // Получение ролей для каждого пользователя
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
            user.Roles = roles.ToList();
        }

        return Ok(new ApiResponse<List<UserDto>>(true, "Список пользователей получен.", users));
    }

    /// <summary>
    /// Назначение роли пользователю.
    /// </summary>
    /// <param name="model">Модель назначения роли.</param>
    /// <returns></returns>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные данные при назначении роли.");
            return BadRequest(new ApiResponse<string>(false, "Некорректные данные."));
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            _logger.LogWarning($"Пользователь с ID {model.UserId} не найден.");
            return NotFound(new ApiResponse<string>(false, "Пользователь не найден."));
        }

        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            _logger.LogWarning($"Роль {model.Role} не существует.");
            return NotFound(new ApiResponse<string>(false, "Роль не существует."));
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
            _logger.LogInformation($"Роль {model.Role} успешно назначена пользователю {user.UserName}.");
            return Ok(new ApiResponse<string>(true, $"Роль {model.Role} успешно назначена пользователю."));
        }
        else
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка при назначении роли: {errors}");
            return BadRequest(new ApiResponse<string>(false, $"Ошибка при назначении роли: {errors}"));
        }
    }

    /// <summary>
    /// Удаление роли у пользователя.
    /// </summary>
    /// <param name="model">Модель удаления роли.</param>
    /// <returns></returns>
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные данные при удалении роли.");
            return BadRequest(new ApiResponse<string>(false, "Некорректные данные."));
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            _logger.LogWarning($"Пользователь с ID {model.UserId} не найден.");
            return NotFound(new ApiResponse<string>(false, "Пользователь не найден."));
        }

        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            _logger.LogWarning($"Роль {model.Role} не существует.");
            return NotFound(new ApiResponse<string>(false, "Роль не существует."));
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
            _logger.LogInformation($"Роль {model.Role} успешно удалена у пользователя {user.UserName}.");
            return Ok(new ApiResponse<string>(true, $"Роль {model.Role} успешно удалена у пользователя."));
        }
        else
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка при удалении роли: {errors}");
            return BadRequest(new ApiResponse<string>(false, $"Ошибка при удалении роли: {errors}"));
        }
    }
}