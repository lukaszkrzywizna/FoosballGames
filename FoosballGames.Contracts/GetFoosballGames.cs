using System.Collections.Generic;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames.Contracts
{
    public record GetFoosballGames : IQuery<FoosballGamesResponse>
    {
    }

    public record FoosballGamesResponse(IReadOnlyCollection<FoosballGame> FoosballGames);
}