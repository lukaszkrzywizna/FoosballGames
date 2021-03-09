using System.Threading.Tasks;

namespace FoosballGames.Infrastructure.Messaging
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}