namespace FoosballGames.Infrastructure.Messaging

open System.Threading.Tasks

type ICommandHandler<'TCommand when 'TCommand :> ICommand> =
    abstract member HandleAsync: command : 'TCommand -> Task
