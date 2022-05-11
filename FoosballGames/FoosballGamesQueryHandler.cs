using FoosballGames.Contracts;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames;

public class FoosballGamesQueryHandler : IQueryHandler<GetFoosballGames, FoosballGamesResponse>,
    IQueryHandler<GetFoosballGame, Contracts.FoosballGame>
{
    private readonly IFoosballGamesRepository _foosballGamesRepository;

    public FoosballGamesQueryHandler(IFoosballGamesRepository foosballGamesRepository)
    {
        _foosballGamesRepository = foosballGamesRepository;
    }

    public async Task<FoosballGamesResponse> HandleAsync(GetFoosballGames query)
    {
        var results = await _foosballGamesRepository.GetAll();
        var ordered = results.Select(x => x.ToContract()).OrderByDescending(s => s.Start).ToArray();
        return new FoosballGamesResponse(ordered);
    }

    public async Task<Contracts.FoosballGame> HandleAsync(GetFoosballGame query)
    {
        var result = await _foosballGamesRepository.Get(query.Id);
        return result.ToContract();
    }
}