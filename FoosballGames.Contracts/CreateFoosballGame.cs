using System;
using FoosballGames.Infrastructure.Messaging;

namespace FoosballGames.Contracts
{
    public record CreateFoosballGame(Guid Id, DateTime Start) : ICommand;
}