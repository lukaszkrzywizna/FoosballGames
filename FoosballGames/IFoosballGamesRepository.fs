namespace FoosballGames

open System
open System.Collections.Generic
open System.Threading.Tasks
open FoosballGames
open FoosballGames.Contracts.Exceptions
open Newtonsoft.Json
open NodaTime
open NodaTime.Serialization.JsonNet
open Microsoft.EntityFrameworkCore
open Npgsql
    
type FoosballGamesRepository(ctx: FoosballGamesContext) =
    let (|PostgresAlreadyExists|_|) trySQLAlreadyExists (ex: Exception) =
        match ex.InnerException with
        | :? PostgresException as ex when ex.SqlState = "23505" -> Some ()
        | _ -> None
        
    static let settings =
        JsonSerializerSettings(TypeNameHandling = TypeNameHandling.All)
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
            
    member this.GetAll() =
        task {
            let! results = ctx.FoosballGames.ToArrayAsync()
            return results
                   |> Array.map (fun x -> JsonConvert.DeserializeObject<FoosballGame>(x.JsonContent, settings))
                   :> FoosballGame IReadOnlyCollection
        }