using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScholaPlan.API.DTOs;

namespace ScholaPlan.API.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("error")]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            _logger.LogError(exception, "Необработанное исключение.");

            return StatusCode(500, ApiResponse<string>.FailureResponse("Внутренняя ошибка сервера."));
        }
    }
}