using FoosballGames.Contracts;
using FoosballGames.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace FoosballGames.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FoosballGamesController : ControllerBase
{
    private readonly ICommandQueryDispatcher _commandQueryDispatcher;

    public FoosballGamesController(ICommandQueryDispatcher commandQueryDispatcher) => 
        _commandQueryDispatcher = commandQueryDispatcher;

    [HttpGet]
    public Task<FoosballGamesResponse> Get() => 
        _commandQueryDispatcher.SendAsync<GetFoosballGames, FoosballGamesResponse>(new GetFoosballGames());

    [HttpGet("{id:Guid}")]
    public Task<Contracts.FoosballGame> GetById(Guid id) => 
        _commandQueryDispatcher.SendAsync<GetFoosballGame, Contracts.FoosballGame>(new GetFoosballGame(id));

    [HttpPost("create-game")]
    public Task Create([FromBody] CreateFoosballGame cmd) => 
        _commandQueryDispatcher.SendAsync(cmd);

    [HttpPost("add-point")]
    public Task AddPoint([FromBody] AddPointForTeam cmd) => 
        _commandQueryDispatcher.SendAsync(cmd);
}