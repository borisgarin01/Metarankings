namespace Domain.Games.Collections;

public sealed record GameCollectionItem
{
    public long GameId { get; set; }
    public Game Game { get; set; }
    public long CollectionId { get; set; }
    public GameCollection Collection { get; set; }
}
