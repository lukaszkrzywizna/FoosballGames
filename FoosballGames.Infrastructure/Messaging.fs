module FoosballGames.Infrastructure.Messaging

open System.Diagnostics
open System.Threading.Tasks

let measure handle request =
    let sw = Stopwatch.StartNew()
    let result = handle request
    printfn $"{request.GetType().Name} took {sw.ElapsedMilliseconds}ms"
    result

module Commands =
    type HandleCommand<'cmd, 'err> = 'cmd -> Task<Result<unit, 'err>>

    let log handle cmd =
        task {
            printfn $"started handling {cmd.GetType().Name}"
            let! result = handle cmd
            match result with
            | Ok _ -> printfn $"finished handling {cmd.GetType().Name}"
            | Error err -> printfn $"failed handling {cmd.GetType().Name}: {err}"
            return result    
        }

    let handleCommand (handler: HandleCommand<'cmd, 'err>) decorators (command: 'cmd) =
        let decorators = decorators |> Seq.reduce (>>)
        let decorated = handler |> decorators
        decorated command

    let handleFinal cmd =
        Error 5 |> Task.FromResult
        
    let test = handleCommand handleFinal [| measure; log |] "Asdas"
    
module Queries =
    type HandleQuery<'query, 'result> = 'query -> 'result Task