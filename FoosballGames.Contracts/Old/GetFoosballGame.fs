namespace FoosballGames.OldContracts

open System
open System.Collections.Generic
open FoosballGames.Infrastructure.Messaging

type Set = {Number: int; Finished: bool; RedTeamScore: byte; BlueTeamScore: byte}
type FoosballGame =
    {Id: Guid
     Start: DateTime
     End: DateTime Nullable
     Sets: IReadOnlyCollection<Set>
     Finished: bool
     WinnerTeam: Team Nullable}
    
type GetFoosballGame = { Id: Guid } interface IQuery<FoosballGame>