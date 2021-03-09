using System;
using System.Threading.Tasks;
using FoosballGames.Contracts;
using FoosballGames.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace FoosballGames.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoosballGamesController : ControllerBase
    {
        private readonly ICommandQueryDispatcher commandQueryDispatcher;

        public FoosballGamesController(ICommandQueryDispatcher commandQueryDispatcher)
        {
            this.commandQueryDispatcher = commandQueryDispatcher;
        }

        [HttpGet]
        public Task<FoosballGamesResponse> Get()
        {
            return commandQueryDispatcher.SendAsync<GetFoosballGames, FoosballGamesResponse>(new GetFoosballGames());
        }

        [HttpGet("{id:Guid}")]
        public Task<Contracts.FoosballGame> GetById(Guid id)
        {
            return commandQueryDispatcher.SendAsync<GetFoosballGame, Contracts.FoosballGame>(new GetFoosballGame(id));
        }

        [HttpPost("create-game")]
        public Task Create([FromBody] CreateFoosballGame cmd)
        {
            return commandQueryDispatcher.SendAsync(cmd);
        }

        [HttpPost("add-point")]
        public Task AddPoint([FromBody] AddPointForTeam cmd)
        {
            return commandQueryDispatcher.SendAsync(cmd);
        }
    }
}