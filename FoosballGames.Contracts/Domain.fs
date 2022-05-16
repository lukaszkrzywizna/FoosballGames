module FoosballGames.Contracts

open System
open System.Collections.Generic

type Team =
    | Red
    | Blue

type CreateFoosballGame = {Id: Guid; Start: DateTime}
type AddPointForTeam = {GameId: Guid; Team: Team}

type Command =
    | CreateGame of CreateFoosballGame
    | AddPoint of AddPointForTeam

type Error =
    | GameNotFound
    | GameAlreadyExists
    | GameFinished

type HandleCommand = Command -> Error

type Set = {Number: int; Finished: bool; Winner: Team; BlueTeamScore: byte}
type FoosballGame =
    { Id: Guid
      Start: DateTime
      End: Option<DateTime>
      Sets: IReadOnlyCollection<Set>
      Finished: bool
      Winner: Team option }
    
type GetFoosballGame = Guid -> FoosballGame
type GetFoosballGames = Guid seq -> FoosballGame seq

type Queries<'t> =
    | GetGame of (GetFoosballGame -> 't)
    | GetGames of (GetFoosballGames -> 't)