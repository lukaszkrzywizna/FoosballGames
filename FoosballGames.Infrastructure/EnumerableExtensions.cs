using System.Collections.Generic;

namespace FoosballGames.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToSingletonEnumerable<T>(this T value)
        {
            yield return value;
        }
    }
}