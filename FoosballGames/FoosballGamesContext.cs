using Microsoft.EntityFrameworkCore;

namespace FoosballGames
{
    public class FoosballGamesContext : DbContext
    {
        public FoosballGamesContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DbFoosballGame> FoosballGames { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("FoosballGames");

            modelBuilder.Entity<DbFoosballGame>(m =>
            {
                m.HasKey(s => s.Id);
                m.Property(a => a.JsonContent).IsRequired().HasColumnType("json");
            });
        }
    }
}