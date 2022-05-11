namespace FoosballGames.Infrastructure.Messaging;

public interface ICommandQueryDispatcher
{
    Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task<TResponse> SendAsync<TQuery, TResponse>(TQuery query) where TQuery : IQuery<TResponse>;
}

public class CommandQueryDispatcher : ICommandQueryDispatcher
{
    private readonly Func<Type, object> _serviceProvider;

    private static readonly Type CommandHandlerType = typeof(ICommandHandler<>);

    private static readonly Type QueryHandlerType = typeof(IQueryHandler<,>);

    public CommandQueryDispatcher(Func<Type, object> serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var commandType = command.GetType();
        var genericHandlerType = CommandHandlerType.MakeGenericType(commandType);

        var handler = _serviceProvider(genericHandlerType);
        return ((ICommandHandler<TCommand>) handler).HandleAsync(command);
    }

    public Task<TResponse> SendAsync<TQuery, TResponse>(TQuery query) where TQuery : IQuery<TResponse>
    {
        var queryType = query.GetType();
        var responseType = typeof(TResponse);
        var genericHandlerType = QueryHandlerType.MakeGenericType(queryType, responseType);
        var handler = _serviceProvider(genericHandlerType);

        return ((IQueryHandler<TQuery, TResponse>) handler).HandleAsync(query);
    }
}