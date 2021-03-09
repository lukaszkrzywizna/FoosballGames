using FoosballGames.Infrastructure;

namespace FoosballGames.Contracts.Exceptions
{
    public class CannotAddPointToFinishedSet : DomainException
    {
        public CannotAddPointToFinishedSet() : base("Can not add point to finished set.")
        {
        }
    }
}