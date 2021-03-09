using System.Threading.Tasks;

namespace FoosballGames.Infrastructure.Messaging
{
    public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query);
    }
}