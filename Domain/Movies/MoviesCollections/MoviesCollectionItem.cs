namespace Domain.Movies.MoviesCollections;

public sealed record MoviesCollectionItem
{
    public long Id { get; set; }
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long MovieCollectionId { get; set; }
    public MoviesCollection MoviesCollection { get; set; }
}
