module FoosballGames.CompositionRoot

open System
open System.Collections.Generic
open System.Threading.Tasks
open FoosballGames.Contracts
open FoosballGames.Domain
open FoosballGames.Infrastructure
open Microsoft.EntityFrameworkCore
open Newtonsoft.Json
open NodaTime
open NodaTime.Serialization.JsonNet
open Npgsql

type HandleCommand = Messaging.Commands.HandleCommand<Command, Error>
//type HandleQuery = Messaging.Queries.HandleQuery<Queries,  

type Root =
    { HandleCmd: HandleCommand
    }
    
let composeRoot (dbCtx: FoosballGamesContext) =
    let jsonSettings = JsonSerializerSettings(TypeNameHandling = TypeNameHandling.All).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
    
    let gameExists id = dbCtx.FoosballGames.AnyAsync(fun x -> x.Id = id)
    
    let (|PostgresAlreadyExists|_|) trySQLAlreadyExists (ex: Exception) =
        match ex.InnerException with
        | :? PostgresException as ex when ex.SqlState = "23505" -> Some ()
        | _ -> None
    
    let addGame (game: FoosballGame) =
        task {
            let serialized = { DbFoosballGame.Id = game.Id; JsonContent =  JsonConvert.SerializeObject(game)}
            let! _ = dbCtx.FoosballGames.AddAsync(serialized)
            try
                let! _ = dbCtx.SaveChangesAsync()
                return Ok()
            with
            | PostgresAlreadyExists() ->
                return Error GameAlreadyExists
        }
        
    let updateGame (game: FoosballGame) =
        let content = JsonConvert.SerializeObject(game, jsonSettings);
        let dbEntity = dbCtx.FoosballGames.Local |> Seq.filter (fun x -> x.Id = game.Id) |> Seq.exactlyOne
        dbEntity.UpdateContent(content);
        dbCtx.SaveChangesAsync() :> Task
        
    let findGame id =
        task {
            let! result = dbCtx.FoosballGames.SingleOrDefaultAsync(fun x -> x.Id = id)
            //if isNull result then return Error GameNotFound
            return Some result |> Option.map (fun x -> JsonConvert.DeserializeObject<FoosballGame>(x.JsonContent, jsonSettings))   
        }
        
    let getGames ids =
        task {
            let! results = dbCtx.FoosballGames.ToArrayAsync()
            return results
                   |> Array.map (fun x -> JsonConvert.DeserializeObject<FoosballGame>(x.JsonContent, jsonSettings))
                   :> FoosballGame IReadOnlyCollection
        }
        
    
    
    let createGame = FoosballCreationWorkflow.initialize gameExists addGame
    let addPoint = AddingPointWorkflow.addPoint findGame updateGame
    
    let handleCommand (cmd: Command) : Task<Result<unit, Error>> =
        match cmd with
        | CreateGame game -> createGame game
        | AddPoint add -> addPoint add
        
    let getAllGames (q: Guid seq) =
        Unchecked.defaultof<Contracts.FoosballGame>
        |> Seq.singleton
        |> Task.FromResult //getGames q
             
    let getGame (q: Guid) = Task.FromResult Unchecked.defaultof<Contracts.FoosballGame>
    
    let getAllMock (q: Guid seq) = Unchecked.defaultof<Contracts.FoosballGame> |> Array.singleton :> Contracts.FoosballGame IReadOnlyCollection
    let getMock (q: Guid) = Unchecked.defaultof<Contracts.FoosballGame>
    
    let handleQueryInternal (query: Queries<'t>) : 't =
        match query with
        | GetGames g -> getAllGames |> g
        | GetGame id -> getGame |> id
        
    let aa = GetGame
    let bb = handleQueryInternal (GetGame id)
    let bb f =
        let s = f id
        handleQueryInternal s
        //q |> handleQueryInternal (f id)
    let cc q = bb q
    let _ = cc GetGame (Guid.NewGuid())
    
    let bb f q = q |> (handleQueryInternal f)
    let cc f q = bb (f(id)) q
    
    let query = GetGame(fun q -> q(Guid.NewGuid()))
    let dd = handleQueryInternal query
    
    //handleQueryInternal (GetGame)
    
    let hq f q = q |> handleQueryInternal (f id)
    let hh f q = f(fun x -> x(q)) |> handleQueryInternal
    let tst f q = q |> f
    
    let hh1 (f: ('b -> 'b) -> Queries<'c>) = f(id) |> handleQueryInternal
    let hh111 = hh1 GetGame (Guid.NewGuid())
    
    let hh (f: (('query -> 'result) -> 'result) -> Queries<'result> ) (q: 'query) = f(fun x -> x(q)) |> handleQueryInternal
    
    // ('a -> 'Queries<_>)
    // 'a: ('b -> 'c)
    
    let asd = handleQueryInternal (GetGame(fun x -> x(Guid.NewGuid())))
    
    let aaaaaaaa = hh GetGame (Guid.NewGuid())
    
    //let handleQuery (f: 'i -> Queries<'t>) (q: 'i) = q |> handleQueryInternal (f id) 
    //let a = handleQueryInternal (GetGame(id))//  handleQuery GetGame (Guid.NewGuid())
    
    { HandleCmd = handleCommand
      HandleQuery = handleQuery }