using System;

namespace FoosballGames.Infrastructure
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}