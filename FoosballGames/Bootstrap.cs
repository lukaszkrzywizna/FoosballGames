using FoosballGames.Contracts;
using FoosballGames.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace FoosballGames
{
    public static class Bootstrap
    {
        public static IServiceCollection RegisterFoosballGamesComponents(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IQueryHandler<GetFoosballGames, FoosballGamesResponse>, FoosballGamesQueryHandler>()
                .AddScoped<IQueryHandler<GetFoosballGame, Contracts.FoosballGame>, FoosballGamesQueryHandler>()
                .AddScoped<ICommandHandler<AddPointForTeam>, FoosballGameCommandHandler>()
                .AddScoped<ICommandHandler<CreateFoosballGame>, FoosballGameCommandHandler>();
        }
    }
}