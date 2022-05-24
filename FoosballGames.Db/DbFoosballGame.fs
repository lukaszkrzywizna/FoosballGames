namespace FoosballGames

open System

//[<AllowNullLiteral>]
[<CLIMutable>]
type DbFoosballGame =
    {Id: Guid; mutable JsonContent: string}
    member this.UpdateContent content =
        this.JsonContent <- content