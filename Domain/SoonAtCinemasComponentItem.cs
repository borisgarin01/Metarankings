namespace Domain;
public sealed record SoonAtCinemasComponentItem
{
    public string Href { get; set; }
    public string ImageSrc { get; set; }
    public string Name { get; set; }
    public string OriginalName { get; set; }
    public DateTime ReleaseDate { get; set; }
}
