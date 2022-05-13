namespace FoosballGames.Infrastructure.Messaging

open System
open System.Threading.Tasks
open FoosballGames.Infrastructure.Messaging

type ICommandQueryDispatcher =
    abstract member SendAsync: command: 'TCommand -> Task when 'TCommand :> ICommand
    abstract member SendAsync: query: 'TQuery -> Task<'TResponse> when 'TQuery :> IQuery<'TResponse>
    
type CommandQueryDispatcher(serviceProvider: Type -> obj) =
    let _serviceProvider = serviceProvider
    static let CommandHandlerType = typedefof<ICommandHandler<_>>
    static let QueryHandlerType = typedefof<IQueryHandler<_,_>>
    
    interface ICommandQueryDispatcher with
        member this.SendAsync<'TCommand when 'TCommand :> ICommand>(command: 'TCommand): Task =
            let commandType = command.GetType()
            let genericHandlerType = CommandHandlerType.MakeGenericType(commandType)
            let handler = _serviceProvider(genericHandlerType)
            (handler :?> ICommandHandler<'TCommand>).HandleAsync(command)

        member this.SendAsync<'TQuery, 'TResponse when 'TQuery :> IQuery<'TResponse>>(query: 'TQuery): Task<'TResponse> =
            let queryType = query.GetType();
            let responseType = typedefof<'TResponse>;
            let genericHandlerType = QueryHandlerType.MakeGenericType(queryType, responseType);
            let handler = _serviceProvider(genericHandlerType);
            (handler :?> IQueryHandler<'TQuery, 'TResponse>).HandleAsync(query);