using FoosballGames.Contracts.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Npgsql;

namespace FoosballGames;

public interface IFoosballGamesRepository
{
    public Task<FoosballGame> Get(Guid id);
    public Task<IReadOnlyCollection<FoosballGame>> GetAll();
    public Task Add(FoosballGame game);
    public Task Update(FoosballGame game);
}

public class FoosballGamesRepository : IFoosballGamesRepository
{
    private readonly FoosballGamesContext _context;

    private static readonly JsonSerializerSettings Settings =
        new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    public FoosballGamesRepository(FoosballGamesContext context)
    {
        _context = context;
    }

    public async Task<FoosballGame> Get(Guid id)
    {
        var result = await _context.FoosballGames.SingleOrDefaultAsync(s => s.Id == id);
        if (result is null) throw new FoosballGameNotFound();
        var content = JsonConvert.DeserializeObject<FoosballGame>(result.JsonContent, Settings);
        return content!;
    }

    public async Task<IReadOnlyCollection<FoosballGame>> GetAll()
    {
        var results = await _context.FoosballGames.ToArrayAsync();
        return results.Select(x => JsonConvert.DeserializeObject<FoosballGame>(x.JsonContent, Settings)!).ToArray();
    }

    public async Task Add(FoosballGame game)
    {
        var content = JsonConvert.SerializeObject(game, Settings);
        var dbGame = new DbFoosballGame(game.Id, content);
        await _context.AddAsync(dbGame);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex.InnerException is PostgresException {SqlState: "23505"})
        {
            throw new FoosballGameAlreadyExists();
        }
    }

    public async Task Update(FoosballGame game)
    {
        var content = JsonConvert.SerializeObject(game, Settings);
        var dbEntity = _context.FoosballGames.Local.Single(s => s.Id == game.Id);
        dbEntity.UpdateContent(content);
        await _context.SaveChangesAsync();
    }
}