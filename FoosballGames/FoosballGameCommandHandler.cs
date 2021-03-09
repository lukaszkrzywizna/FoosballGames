using System.Threading.Tasks;
using FoosballGames.Contracts;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames
{
    public class FoosballGameCommandHandler : ICommandHandler<AddPointForTeam>, ICommandHandler<CreateFoosballGame>
    {
        private readonly IFoosballGamesRepository foosballGamesRepository;

        public FoosballGameCommandHandler(IFoosballGamesRepository foosballGamesRepository)
        {
            this.foosballGamesRepository = foosballGamesRepository;
        }

        public async Task HandleAsync(AddPointForTeam command)
        {
            var game = await foosballGamesRepository.Get(command.GameId);
            game.AddPointForTeam(command.ForBlueTeam);
            await foosballGamesRepository.Update(game);
        }

        public async Task HandleAsync(CreateFoosballGame command)
        {
            var game = FoosballGame.Initialize(command);
            await foosballGamesRepository.Add(game);
        }
    }
}