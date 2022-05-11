using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames.Contracts;

public record GetFoosballGame(Guid Id) : IQuery<FoosballGame>;

public record FoosballGame
(
    Guid Id,
    DateTime Start,
    DateTime? End,
    IReadOnlyCollection<Set> Sets,
    bool Finished,
    Team? WinnerTeam
);

public record Set(int Number, bool Finished, byte RedTeamScore, byte BlueTeamScore);