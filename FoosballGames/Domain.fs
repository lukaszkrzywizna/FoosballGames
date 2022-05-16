module FoosballGames.Domain

open System
open FoosballGames.Contracts
open NodaTime

type RunningSet =
    {BlueTeamScore: byte; RedTeamScore: byte}
    static member New = {BlueTeamScore = 0uy; RedTeamScore = 0uy}
    
type FinishedSet =
    {BlueTeamScore: byte; RedTeamScore: byte; Winner: Team}

type Set =
    | Running of RunningSet
    | Finished of FinishedSet

type SecondSetRunning =
    {FirstSet: FinishedSet; SecondSet: RunningSet}
        
type ThirdSetRunning = {FirstSet: FinishedSet; SecondSet: FinishedSet; ThirdSet: RunningSet}
type FinishedSets =
    {FirstSet: FinishedSet; SecondSet: FinishedSet; ThirdSet: FinishedSet option; WinnerTeam: Team}

type Game =
    | FirstSet of RunningSet
    | SecondSet of SecondSetRunning
    | ThirdSet of ThirdSetRunning
    | FinishedGame of FinishedSets

type FoosballGame = { Id: Guid; Start: LocalDateTime; End: LocalDateTime option; Sets: Game }