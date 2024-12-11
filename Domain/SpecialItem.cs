namespace Domain;
public sealed record SpecialItem
{
    public string Href { get; set; }
    public string Title { get; set; }
    public string ImageAlt { get; set; }
    public string ImageSrc { get; set; }
}
