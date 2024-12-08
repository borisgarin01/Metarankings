namespace Domain;
public sealed record GameLocalization
{
    public long Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public IEnumerable<Game> Games { get; set; }
}
