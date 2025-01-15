using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScholaPlan.API.DTOs;
using ScholaPlan.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScholaPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IConfiguration configuration,
    ILogger<AuthController> logger)
    : ControllerBase
{
    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="model">Модель регистрации.</param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Некорректные данные при регистрации пользователя.");
            return BadRequest(new ApiResponse<string>(false, "Некорректные данные."));
        }

        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            logger.LogWarning($"Пользователь с именем {model.Username} уже существует.");
            return BadRequest(new ApiResponse<string>(false, "Пользователь с таким именем уже существует."));
        }

        ApplicationUser user = new ApplicationUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            logger.LogWarning("Ошибка при создании пользователя.");
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(new ApiResponse<string>(false, $"Ошибка при создании пользователя: {errors}"));
        }

        // Назначение роли "User" по умолчанию
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new ApplicationRole { Name = "User" });

        await userManager.AddToRoleAsync(user, "User");

        logger.LogInformation($"Пользователь {model.Username} успешно зарегистрирован.");
        return Ok(new ApiResponse<string>(true, "Пользователь успешно зарегистрирован."));
    }

    /// <summary>
    /// Аутентификация пользователя и получение JWT токена.
    /// </summary>
    /// <param name="model">Модель логина.</param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Некорректные данные при входе пользователя.");
            return BadRequest(new ApiResponse<string>(false, "Некорректные данные."));
        }

        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            logger.LogWarning($"Пользователь с именем {model.Username} не найден.");
            return Unauthorized(new ApiResponse<string>(false, "Неверные учетные данные."));
        }

        if (!await userManager.CheckPasswordAsync(user, model.Password))
        {
            logger.LogWarning($"Неверный пароль для пользователя {model.Username}.");
            return Unauthorized(new ApiResponse<string>(false, "Неверные учетные данные."));
        }

        var userRoles = await userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = GetToken(authClaims);

        logger.LogInformation($"Пользователь {model.Username} успешно аутентифицирован.");

        return Ok(new ApiResponse<string>(true, "Успешный вход.", new JwtSecurityTokenHandler().WriteToken(token)));
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.UtcNow.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}