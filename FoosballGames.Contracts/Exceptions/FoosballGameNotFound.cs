using FoosballGames.Infrastructure;

namespace FoosballGames.Contracts.Exceptions
{
    public class FoosballGameNotFound : DomainException
    {
        public FoosballGameNotFound() : base("Foosball game not found.")
        {
        }
    }
}