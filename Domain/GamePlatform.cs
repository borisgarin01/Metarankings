namespace Domain;
public sealed record GamePlatform
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public IEnumerable<Game> Games { get; set; }
}
