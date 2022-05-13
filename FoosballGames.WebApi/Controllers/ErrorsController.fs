namespace FoosballGames.WebApi.Controllers

open FoosballGames.Infrastructure
open Microsoft.AspNetCore.Diagnostics
open Microsoft.AspNetCore.Mvc

[<ApiExplorerSettings(IgnoreApi = true)>]
type ErrorsController() =
    inherit ControllerBase()
    
    [<Route("error")>]
    member this.Error() =
        let ctx = this.HttpContext.Features.Get<IExceptionHandlerFeature>()
        let ``exception`` = ctx.Error
        let code = if ``exception`` :? DomainException then 400 else 500
        this.Problem(ctx.Error.Message, statusCode = code)