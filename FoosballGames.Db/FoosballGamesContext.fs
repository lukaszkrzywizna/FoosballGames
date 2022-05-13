namespace FoosballGames

open FoosballGames
open Microsoft.EntityFrameworkCore

type FoosballGamesContext(options: DbContextOptions) =
    inherit DbContext(options)
    
    [<DefaultValue>] val mutable foosballGames: DbSet<DbFoosballGame> 
    member this.FoosballGames
        with get () = this.foosballGames
        and set value = this.foosballGames <- value
    
    override this.OnModelCreating(builder: ModelBuilder) =
        base.OnModelCreating(builder)
        builder
            .HasDefaultSchema("FoosballGames")
            .Entity<DbFoosballGame>(
                fun m ->
                    m.HasKey(fun x -> x.Id :> obj) |> ignore
                    m.Property(fun s -> s.JsonContent :> obj).HasColumnType("json") |> ignore
                )
            |> ignore