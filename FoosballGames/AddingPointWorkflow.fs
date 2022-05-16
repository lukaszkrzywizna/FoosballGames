module FoosballGames.AddingPointWorkflow

open System
open System.Threading.Tasks
open FoosballGames.Contracts
open FoosballGames.Domain

let [<Literal>] MaxPointsScore = 10uy

type AddingPointResult =
    | Finished of FinishedSet
    | Running of RunningSet

let (|OneIsTrue|_|) oneOfIsTrue (a,b) = if a = true || b = true then Some () else None
    
let private addPointToSet (runningSet: RunningSet) (team: Team): AddingPointResult =
    let blueScore, redScore =
        match team with
        | Team.Blue -> (runningSet.BlueTeamScore + 1uy, runningSet.RedTeamScore)
        | Team.Red -> (runningSet.BlueTeamScore, runningSet.RedTeamScore + 1uy)
    
    match (blueScore = MaxPointsScore, redScore = MaxPointsScore) with
    | OneIsTrue () ->
        Finished {RedTeamScore = redScore; BlueTeamScore = blueScore; Winner = team}
    | _ ->
        Running {RedTeamScore = redScore; BlueTeamScore = blueScore}    

let addPointToGame (game: FoosballGame option) (cmd: AddPointForTeam) : Result<FoosballGame, Error> =
    let addPoint s = addPointToSet s cmd.Team

    match game with
    | None -> Error GameNotFound
    | Some game ->
        match game.Sets with
        | FinishedGame _ -> Error GameFinished
        | FirstSet set ->
            let sets =
                match addPoint set with
                | Finished f -> SecondSet { FirstSet = f; SecondSet = RunningSet.New }
                | Running r -> FirstSet r
            {game with Sets = sets} |> Ok
        | SecondSet s ->
            let sets =
                match addPoint s.SecondSet with
                | Finished f when f.Winner = s.FirstSet.Winner ->
                    FinishedGame { FirstSet = s.FirstSet; SecondSet = f; ThirdSet = None; WinnerTeam = f.Winner }
                | Finished f ->
                    ThirdSet { FirstSet = s.FirstSet; SecondSet = f; ThirdSet = RunningSet.New; }
                | Running r -> SecondSet {s with SecondSet = r}
            {game with Sets = sets} |> Ok
        | ThirdSet t ->
            let sets =
                match addPoint t.ThirdSet with
                | Finished f ->
                    FinishedGame { FirstSet = f; SecondSet = t.SecondSet; ThirdSet = Some(f); WinnerTeam = f.Winner }
                | Running r ->
                    ThirdSet { t with ThirdSet = r }
            {game with Sets = sets} |> Ok    

let addPoint (findGame: Guid -> FoosballGame option Task) (cmd: AddPointForTeam) : Task<Result<FoosballGame, Error>> =
    task {
        let! game = findGame cmd.GameId
        return addPointToGame game cmd
    }