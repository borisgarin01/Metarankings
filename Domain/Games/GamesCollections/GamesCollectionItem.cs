namespace Domain.Games.Collections;

public sealed record GamesCollectionItem
{
    public long Id { get; set; }
    public long GameId { get; set; }
    public Game Game { get; set; }
    public long GamesCollectionId { get; set; }
    public GamesCollection GamesCollection { get; set; }
}
