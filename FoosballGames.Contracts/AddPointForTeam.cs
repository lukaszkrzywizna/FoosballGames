using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames.Contracts;

public enum Team { Blue, Red}
public record AddPointForTeam(Guid GameId, Team Team) : ICommand;