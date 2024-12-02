namespace Domain;
public sealed record NewsComponentItem
{
    public string LinkHref { get; set; }
    public string LinkTitle { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
}
