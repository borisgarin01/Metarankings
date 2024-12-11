namespace Domain;
public sealed record NewMovie
{
    public string Description { get; set; }
    public string ImageHref { get; set; }
    public string Link { get; set; }
    public DateTime ReleaseDate { get; set; }
    public float Score { get; set; }
    public string Title { get; set; }
}
