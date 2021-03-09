using System;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames.Contracts
{
    public record AddPointForTeam(Guid GameId, bool ForBlueTeam) : ICommand;
}