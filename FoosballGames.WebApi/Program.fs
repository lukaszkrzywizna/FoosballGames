module FoosballGames.WebApi.Program
#nowarn "20"

open FoosballGames
open FoosballGames.Infrastructure.Messaging
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Options
open FoosballGames.Bootstrap
open System.Reflection
open Microsoft.EntityFrameworkCore

let exitCode = 0

[<EntryPoint>]
let main args =

    let builder =
        WebApplication.CreateBuilder(args)

    let services = builder.Services
    
    services.AddControllers()

    services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .Configure<PostgreSqlSettings>(builder.Configuration.GetSection(nameof(PostgreSqlSettings)))
        .AddScoped<ICommandQueryDispatcher>(fun sp -> CommandQueryDispatcher(sp.GetRequiredService))
        .RegisterFoosballGamesComponents()
        .AddDbContext<FoosballGamesContext>(fun sp opts ->
            let settings = sp.GetRequiredService<IOptions<PostgreSqlSettings>>().Value.BuildConnectionString()
            opts.UseNpgsql(settings, fun b ->
                b.MigrationsAssembly(typedefof<FoosballGamesContext>.GetTypeInfo().Assembly.FullName) |> ignore)
            |> ignore)
        .AddScoped<IFoosballGamesRepository, FoosballGamesRepository>()
    
    let app = builder.Build()

    let root = CompositionRoot.composeRoot(app.Services)
    
    if app.Environment.IsDevelopment()
    then
        app.UseSwagger()
        app.UseSwaggerUI() |> ignore
    else
        app.UseExceptionHandler("/error") |> ignore
        
    app.UseHttpsRedirection()

    app.MapControllers()

    //app.MapGet("/", fun ctx -> ctx.Response.Redirect("/swagger"))
    
    app.Run()

    exitCode
