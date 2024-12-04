namespace Domain;

public sealed record GamesIndexComponentItem
{
    public IndexComponentItem IndexComponentItem { get; set; }
    public IEnumerable<GamePlatform> Platforms { get; set; }
}
