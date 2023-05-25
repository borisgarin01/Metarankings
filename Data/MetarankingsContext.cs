using Metarankings.Models;
using Microsoft.EntityFrameworkCore;

namespace Metarankings.Data
{
    public class MetarankingsContext:DbContext
    {
        public MetarankingsContext(DbContextOptions<MetarankingsContext> options):base(options)
        {
            Database.Migrate();
        }

        public DbSet<Critic> Critics { get; set; }
        public DbSet<CriticReview> CriticsReviews { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Game> GamesGenres { get; set; }
        public DbSet<GamePlatform> GamesPlatforms { get; set; }
        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<GamerReview> GamersReviews { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<ScreetshotUrl> ScreetshotsUrls { get; set; }
        public DbSet<TrailerUrl> TrailersUrls { get; set; }
        public DbSet<VideoreviewUrl> VideoreviewsUrls { get; set; }
    }
}
