namespace Domain.Games;

public sealed record PlatformGame
{
    public long Id { get; set; }
    public long GameId { get; set; }
    public long PlatformId { get; set; }
}
