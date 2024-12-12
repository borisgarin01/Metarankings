namespace Domain;

public sealed record GameReleaseDate
{
    public string Href { get; set; }
    public string ImageSrc { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
}
