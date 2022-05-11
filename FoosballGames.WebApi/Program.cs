using System.Reflection;
using FoosballGames;
using FoosballGames.Infrastructure.Messaging;
using FoosballGames.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<PostgreSqlSettings>(builder.Configuration.GetSection(nameof(PostgreSqlSettings)));
services.AddScoped<ICommandQueryDispatcher>(sp => new CommandQueryDispatcher(sp.GetRequiredService));
services.RegisterFoosballGamesComponents();
services.AddDbContext<FoosballGamesContext>((sp, options) =>
{
    var settings = sp.GetRequiredService<IOptions<PostgreSqlSettings>>().Value.BuildConnectionString();
    options.UseNpgsql(settings,
        b => b.MigrationsAssembly(typeof(FoosballGamesContext).GetTypeInfo().Assembly.FullName));
});
services.AddScoped<IFoosballGamesRepository, FoosballGamesRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();