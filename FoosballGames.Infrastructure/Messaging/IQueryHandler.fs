namespace FoosballGames.Infrastructure.Messaging

open System.Threading.Tasks

type IQueryHandler<'TQuery, 'TResult when 'TQuery :> IQuery<'TResult>> =
    abstract member HandleAsync: query: 'TQuery -> Task<'TResult>