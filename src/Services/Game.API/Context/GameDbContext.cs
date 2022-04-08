using Microsoft.EntityFrameworkCore;

namespace Game_API.Context
{
    public class GameDbContext : DbContext
    {
        public DbSet<PlayedGame> PlayedGames => Set<PlayedGame>();

        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlayedGameEntityTypeConfiguration());
        }
    }
}
