namespace Domain;
public sealed record News
{
    public string Href { get; set; }
    public string Title { get; set; }
    public string ImageSrc { get; set; }
}
