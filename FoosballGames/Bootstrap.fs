module FoosballGames.Bootstrap

open FoosballGames.Contracts
open FoosballGames.Infrastructure.Messaging
open Microsoft.Extensions.DependencyInjection

type IServiceCollection with
    member this.RegisterFoosballGamesComponents() =
        this
            .AddScoped<IQueryHandler<GetFoosballGames, FoosballGamesResponse>, FoosballGamesQueryHandler>()
            .AddScoped<IQueryHandler<GetFoosballGame, Contracts.FoosballGame>, FoosballGamesQueryHandler>()
            .AddScoped<ICommandHandler<AddPointForTeam>, FoosballGameCommandHandler>()
            .AddScoped<ICommandHandler<CreateFoosballGame>, FoosballGameCommandHandler>();
    