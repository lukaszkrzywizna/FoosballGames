namespace FoosballGames

open System

[<CLIMutable>]
type DbFoosballGame =
    {Id: Guid; mutable JsonContent: string}
    member this.UpdateContent content =
        this.JsonContent <- content