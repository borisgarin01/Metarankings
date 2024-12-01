namespace Domain;
public sealed record GamesNewsComponentItem
{
    public string LinkHref { get; set; }
    public string LinkTitle { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
}
