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
    public DbSet<GameGameGenre> GamesGamesGenres { get; set; }
    public DbSet<GameGameDeveloper> GameGameDevelopers { get; set; }
    public DbSet<GameGamePlatform> GameGamePlatforms { get; set; }
    public DbSet<GameGamePublisher> GameGamePublishers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Developers)
            .WithMany(d => d.Games)
            .UsingEntity<GameGameDeveloper>(
                j => j.HasOne(gd => gd.GameDeveloper).WithMany(),
                j => j.HasOne(gd => gd.Game).WithMany());

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Publishers)
            .WithMany(p => p.Games)
            .UsingEntity<GameGamePublisher>(
                j => j.HasOne(gp => gp.GamePublisher).WithMany(),
                j => j.HasOne(gp => gp.Game).WithMany());

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Genres)
            .WithMany(gn => gn.Games)
            .UsingEntity<GameGameGenre>(
                j => j.HasOne(gg => gg.GameGenre).WithMany(),
                j => j.HasOne(gg => gg.Game).WithMany());

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Platforms)
            .WithMany(gn => gn.Games)
            .UsingEntity<GameGamePlatform>(
                j => j.HasOne(gg => gg.GamePlatform).WithMany(),
                j => j.HasOne(gg => gg.Game).WithMany());
    }

}
