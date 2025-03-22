namespace Domain;
public sealed record Game
{
    public long Id { get; set; }
    public required string Href { get; set; }
    public required string Name { get; set; }
    public required string Image { get; set; }
    public float? Score { get; set; }
    public long? ScoresCount { get; set; }
    public required string[] Developers { get; set; }
    public required string Publisher { get; set; }
    public required string[] Platforms { get; set; }
    public required string[] Genres { get; set; }
    public required string Localization { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public required string Description { get; set; }
    public string Trailer { get; set; }
    public Screenshot[] Screenshots { get; set; }
}
