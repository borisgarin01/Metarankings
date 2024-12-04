namespace Domain;

public sealed record MoviesIndexComponentItem
{
    public IndexComponentItem IndexComponentItem { get; set; }
    public Genre[] MoviesGenres { get; set; }
}
