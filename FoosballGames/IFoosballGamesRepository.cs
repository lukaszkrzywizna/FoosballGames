using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoosballGames.Contracts.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace FoosballGames
{
    public interface IFoosballGamesRepository
    {
        public Task<FoosballGame> Get(Guid id);
        public Task<IReadOnlyCollection<FoosballGame>> GetAll();
        public Task Add(FoosballGame game);
        public Task Update(FoosballGame game);
    }

    public class FoosballGamesRepository : IFoosballGamesRepository
    {
        private readonly FoosballGamesContext context;

        private static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        public FoosballGamesRepository(FoosballGamesContext context)
        {
            this.context = context;
        }

        public async Task<FoosballGame> Get(Guid id)
        {
            var result = await context.FoosballGames.SingleOrDefaultAsync(s => s.Id == id);
            if (result is null) throw new FoosballGameNotFound();
            var content = JsonConvert.DeserializeObject<FoosballGame>(result.JsonContent, Settings);
            return content!;
        }

        public async Task<IReadOnlyCollection<FoosballGame>> GetAll()
        {
            var results = await context.FoosballGames.ToArrayAsync();
            return results.Select(x => JsonConvert.DeserializeObject<FoosballGame>(x.JsonContent)).ToArray();
        }

        public async Task Add(FoosballGame game)
        {
            var content = JsonConvert.SerializeObject(game, Settings);
            var dbGame = new DbFoosballGame(game.Id, content);
            await context.AddAsync(dbGame);
            await context.SaveChangesAsync();
        }

        public async Task Update(FoosballGame game)
        {
            var content = JsonConvert.SerializeObject(game, Settings);
            var dbEntity = context.FoosballGames.Local.Single(s => s.Id == game.Id);
            dbEntity.UpdateContent(content);
            await context.SaveChangesAsync();
        }
    }
}