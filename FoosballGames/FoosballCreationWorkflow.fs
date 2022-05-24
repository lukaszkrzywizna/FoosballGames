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
    (addGame: FoosballGame -> Task<Result<unit, Error>>)
    (cmd: CreateFoosballGame)
    : Task<Result<unit, Error>> =

    task {
        let! exists = gameExists cmd.Id

        return!
            match initializeGame exists cmd with
            | Ok game -> task { return! addGame game }
            | Error error -> Error error |> Task.FromResult
    }
