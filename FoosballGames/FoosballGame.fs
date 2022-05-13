namespace FoosballGames

open System
open FoosballGames.Contracts
open FoosballGames.Contracts.Exceptions
open NodaTime

type ISet =
    interface end

type FinishedSet =
    {RedTeamScore: byte; BlueTeamScore: byte; WinnerTeam: Team}
    interface ISet

type RunningSet =
    {RedTeamScore: byte; BlueTeamScore: byte}
    interface ISet
    static member New () = {RedTeamScore = 0uy; BlueTeamScore = 0uy}
    static member MaxPointsScore = 10uy
    member this.AddPointForTeam team : ISet =
        let blueScore, redScore =
            match team with
            | Team.Blue -> (this.BlueTeamScore + 1uy, this.RedTeamScore)
            | Team.Red -> (this.BlueTeamScore, this.RedTeamScore + 1uy)
            | _ -> raise <| ArgumentOutOfRangeException(nameof team, team, "Unexpected team.")
        
        match (blueScore = RunningSet.MaxPointsScore, redScore = RunningSet.MaxPointsScore) with
        | true, _ | _, true ->
            {RedTeamScore = redScore; BlueTeamScore = blueScore; WinnerTeam = team}
        | _ -> {RedTeamScore = redScore; BlueTeamScore = blueScore} :> ISet

type ISets =
    abstract member AddPointForTeam: team: Team -> ISets

type FinishedSets =
    {FirstSet: FinishedSet; SecondSet: FinishedSet; ThirdSet: FinishedSet option; WinnerTeam: Team}
    
    interface ISets with
        member this.AddPointForTeam _ : ISets =
            raise <| CannotAddPointToFinishedSet()

type ThirdSetRunning =
    {FirstSet: FinishedSet; SecondSet: FinishedSet; ThirdSet: RunningSet}
    
    interface ISets with
        member this.AddPointForTeam team =
            let currentSet = this.ThirdSet.AddPointForTeam team
            match currentSet with
            | :? FinishedSet as finished ->
                { FirstSet = this.FirstSet; SecondSet = this.SecondSet
                  ThirdSet = Some(finished); WinnerTeam = finished.WinnerTeam }
            | :? RunningSet as running -> { this with ThirdSet = running }
            | _ -> raise <| ArgumentOutOfRangeException(nameof currentSet, currentSet, "Unexpected set type.")    

type SecondSetRunning =
    {FirstSet: FinishedSet; SecondSet: RunningSet}
    interface ISets with
        member this.AddPointForTeam team =
            let currentSet = this.SecondSet.AddPointForTeam team
            match currentSet with
            | :? FinishedSet as finished when finished.WinnerTeam <> this.FirstSet.WinnerTeam ->
                { FirstSet = this.FirstSet; SecondSet = finished; ThirdSet = RunningSet.New() }
            | :? FinishedSet as finished when finished.WinnerTeam = this.FirstSet.WinnerTeam ->
                { FirstSet = this.FirstSet; SecondSet = finished; ThirdSet = None; WinnerTeam = finished.WinnerTeam }
            | :? RunningSet as running -> { this with SecondSet = running }
            | _ -> raise <| ArgumentOutOfRangeException(nameof currentSet, currentSet, "Unexpected set type.")
            
type FirstSetRunning =
    {Set: RunningSet}
    
    interface ISets with
        member this.AddPointForTeam(team) =
            let currentSet = this.Set.AddPointForTeam team
            match currentSet with
            | :? FinishedSet as finished -> { FirstSet = finished; SecondSet = RunningSet.New() }
            | :? RunningSet as running -> { this with Set = running }
            | _ -> raise <| ArgumentOutOfRangeException(nameof currentSet, currentSet, "Unexpected set type.")

type FoosballGame =
    {Id: Guid; Start: LocalDateTime; End: LocalDateTime option; Sets: ISets}
    
    static member Initialize (command: CreateFoosballGame) =
        {Id = command.Id
         Start = LocalDateTime.FromDateTime(command.Start)
         End = None
         Sets = { Set = RunningSet.New() }}
        
    member this.AddPointForTeam team =
        let sets = this.Sets.AddPointForTeam team
        let endTime = if sets :? FinishedSets then Some(LocalDateTime.FromDateTime(DateTime.Now)) else None
        { this with Sets = sets; End = endTime }

(*
module FoosballGames.FoosballGame

open System
open FoosballGames.Contracts
open NodaTime

type RunningSet = {BlueTeamScore: byte; RedTeamScore: byte}
type FinishedSet = {BlueTeamScore: byte; RedTeamScore: byte; Winner: Team}

type Set =
    | Running of RunningSet
    | Finished of FinishedSet

type Game =
    | FirstSet
    | SecondSet
    | ThirdSet
    | Finished

type FoosballGame = { Id: Guid; Start: LocalDateTime; End: LocalDateTime; }
*)