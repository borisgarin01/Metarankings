namespace Domain;

public sealed record GamesIndexComponentItem
{
    public IndexComponentItem IndexComponentItem { get; set; }
    public Platform[] Platforms { get; set; }
}
