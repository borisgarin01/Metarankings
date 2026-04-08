namespace Domain.Games.Collections;

public sealed record GameCollectionItem
{
    public long Id { get; set; }
    public long GameId { get; set; }
    public Game Game { get; set; }
    public long GameCollectionId { get; set; }
    public GameCollection GameCollection { get; set; }
}
