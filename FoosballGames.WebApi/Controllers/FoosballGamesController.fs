namespace FoosballGames.WebApi.Controllers

open FoosballGames.Infrastructure.Messaging
open Microsoft.AspNetCore.Mvc
open FoosballGames.Contracts

[<ApiController>]
[<Route("[controller]")>]
type FoosballGamesController(commandQueryDispatcher: ICommandQueryDispatcher) =
    inherit ControllerBase()

    [<HttpGet>]
    member _.Get() =
        commandQueryDispatcher.SendAsync(GetFoosballGames())
    
    [<HttpGet("{id:Guid}")>]
    member _.GetById(id) =
        commandQueryDispatcher.SendAsync({ GetFoosballGame.Id = id })

    [<HttpPost("create-game")>]
    member _.Create([<FromBody>] cmd: CreateFoosballGame) =
        commandQueryDispatcher.SendAsync cmd
        
    [<HttpPost("add-point")>]
    member _.AddPoint([<FromBody>] cmd: AddPointForTeam) =
        commandQueryDispatcher.SendAsync cmd