namespace Domain;
public sealed record GameTag
{
    public long Id { get; set; }
    public IEnumerable<Game> Games { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}
