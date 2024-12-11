namespace Domain;
public sealed record NewGame
{
    public string Href { get; set; }
    public string Description { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; }
}
