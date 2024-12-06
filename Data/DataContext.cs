using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<BestInPastMonth> BestInPastMonth { get; set; }
    public DbSet<CollectionsComponentItem> CollectionsComponentItems { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<IndexComponentItem> IndexComponentItems { get; set; }
    public DbSet<LastReviewsComponentItem> LastReviewsComponentItems { get; set; }
    public DbSet<NewsComponentItem> NewsComponentsItems { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<SpecialsComponentItem> SpecialsComponentItems { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<DetailsComponentItem> DetailsComponentsItems { get; set; }
}
