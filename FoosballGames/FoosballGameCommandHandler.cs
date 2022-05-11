using FoosballGames.Contracts;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames;

public class FoosballGameCommandHandler : ICommandHandler<AddPointForTeam>, ICommandHandler<CreateFoosballGame>
{
    private readonly IFoosballGamesRepository _foosballGamesRepository;

    public FoosballGameCommandHandler(IFoosballGamesRepository foosballGamesRepository)
    {
        _foosballGamesRepository = foosballGamesRepository;
    }

    public async Task HandleAsync(AddPointForTeam command)
    {
        var game = await _foosballGamesRepository.Get(command.GameId);
        var newGame = game.AddPointForTeam(command.Team);
        await _foosballGamesRepository.Update(newGame);
    }

    public async Task HandleAsync(CreateFoosballGame command)
    {
        var game = FoosballGame.Initialize(command);
        await _foosballGamesRepository.Add(game);
    }
}