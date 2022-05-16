module FoosballGames.FoosballCreationWorkflow

open System
open System.Threading.Tasks
open FoosballGames.Contracts
open FoosballGames.Domain
open NodaTime

let initializeGame (foosballGameExists: bool) (cmd: CreateFoosballGame) : Result<FoosballGame, Error> =
    if not foosballGameExists then
        { Id = cmd.Id
          Start = LocalDateTime.FromDateTime(cmd.Start)
          End = None
          Sets = FirstSet RunningSet.New }
        |> Ok
    else
        Error GameAlreadyExists

let initialize
    (gameExists: Guid -> bool Task)
    (addGame: FoosballGame -> Task)
    (cmd: CreateFoosballGame)
    : Task<Result<unit, Error>> =

    task {
        let! exists = gameExists cmd.Id
        
        return!
            match initializeGame exists cmd with
            | Ok game ->
                task {
                    let! _ = addGame game
                    return Ok()
                }
            | Error error -> Error error |> Task.FromResult
    }
