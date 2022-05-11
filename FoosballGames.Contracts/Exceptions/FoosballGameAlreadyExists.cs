using FoosballGames.Infrastructure;

namespace FoosballGames.Contracts.Exceptions;

public class FoosballGameAlreadyExists : DomainException
{
    public FoosballGameAlreadyExists() : base("Foosball game already exists.")
    {
    }
}