namespace Domain;
public sealed record SoonAtMoviesComponentItem
{
    public string LinkHref { get; set; }
    public string LinkTitle { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
    public string OriginalName { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public Genre[] MoviesGenres { get; set; }
}
