using System;
using System.Collections.Generic;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames.Contracts
{
    public record GetFoosballGame(Guid Id) : IQuery<FoosballGame>;

    public record FoosballGame
    (
        Guid Id,
        DateTime Start,
        DateTime? End,
        IReadOnlyCollection<Set> Sets,
        bool IsFinished,
        bool? BlueTeamWon
    );

    public record Set(int Number, bool IsFinished, byte ReadTeamScore, byte BlueTeamScore);
}