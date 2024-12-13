using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<Critic> Critics { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameCriticReview> GamesCriticsReviews { get; set; }
    public DbSet<GameDeveloper> GamesDevelopers { get; set; }
    public DbSet<GameGenre> GamesGenres { get; set; }
    public DbSet<GameLocalization> GamesLocalizations { get; set; }
    public DbSet<GamePlatform> GamesPlatforms { get; set; }
    public DbSet<GamePublisher> GamesPublishers { get; set; }
    public DbSet<Gamer> Gamers { get; set; }
    public DbSet<GameTag> GamesTags { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<CollectionItem> CollectionItems { get; set; }
    public DbSet<Trailer> Trailers { get; set; }
}
