namespace Domain;

public sealed record MoviesIndexComponentItem
{
    public IndexComponentItem IndexComponentItem { get; set; }
    public IEnumerable<Genre> MoviesGenres { get; set; }
}
