using FoosballGames.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FoosballGames.WebApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("error")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context!.Error;
        var code = 500;

        if (exception is DomainException) code = 400;

        return Problem(context.Error.Message, statusCode: code);
    }
}