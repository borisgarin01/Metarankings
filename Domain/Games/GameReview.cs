namespace Domain.Games;

public sealed record GameReview
{
    public long Id { get; set; }
    public long GameId { get; set; }
    public long ApplicationUserId { get; set; }
    public string ReviewText { get; set; }
}
