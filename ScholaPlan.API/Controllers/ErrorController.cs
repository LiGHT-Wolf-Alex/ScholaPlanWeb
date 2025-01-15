using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;

namespace ScholaPlan.API.Controllers;

[ApiController]
public class ErrorController(ILogger<ErrorController> logger) : ControllerBase
{
    [HttpGet]
    [Route("error")]
    public IActionResult HandleError()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;

        logger.LogError(exception, "Необработанное исключение.");

        return StatusCode(500, new ApiResponse<string>(false, "Внутренняя ошибка сервера."));
    }
}