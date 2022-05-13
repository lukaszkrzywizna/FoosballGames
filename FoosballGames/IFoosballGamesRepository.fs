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

type IFoosballGamesRepository =
    abstract member Get: id: Guid -> FoosballGame Task
    abstract member GetAll: unit -> FoosballGame IReadOnlyCollection Task
    abstract member Add: game: FoosballGame -> Task
    abstract member Update: game: FoosballGame -> Task
    
type FoosballGamesRepository(ctx: FoosballGamesContext) =
    let (|PostgresAlreadyExists|_|) trySQLAlreadyExists (ex: Exception) =
        match ex.InnerException with
        | :? PostgresException as ex when ex.SqlState = "23505" -> Some ()
        | _ -> None
        
    static let settings =
        JsonSerializerSettings(TypeNameHandling = TypeNameHandling.All)
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
            
    interface IFoosballGamesRepository with
        member this.Get id =
            task {
                let! result = ctx.FoosballGames.SingleOrDefaultAsync(fun s -> s.Id = id)
                if result = Unchecked.defaultof<DbFoosballGame> then raise <| FoosballGameNotFound()
                let content = JsonConvert.DeserializeObject<FoosballGame>(result.JsonContent, settings)
                return content;
            }
        member this.GetAll() =
            task {
                let! results = ctx.FoosballGames.ToArrayAsync()
                return results
                       |> Array.map (fun x -> JsonConvert.DeserializeObject<FoosballGame>(x.JsonContent, settings))
                       :> FoosballGame IReadOnlyCollection
            }
        member this.Add game =
            let content = JsonConvert.SerializeObject(game, settings)
            let dbGame = DbFoosballGame(game.Id, content)
            task {
                let! _ = ctx.FoosballGames.AddAsync(dbGame)
                try
                    return! ctx.SaveChangesAsync() :> Task
                with
                | PostgresAlreadyExists() -> raise (FoosballGameAlreadyExists())
            }
        member this.Update game =
            let content = JsonConvert.SerializeObject(game, settings);
            let dbEntity = ctx.FoosballGames |> Seq.filter (fun x -> x.Id = game.Id) |> Seq.exactlyOne
            dbEntity.UpdateContent(content);
            ctx.SaveChangesAsync();
