namespace Domain;
public sealed record Gamer
{
    public long Id { get; set; }
    public string Url { get; set; }
    public string AccountName { get; set; }
    public IEnumerable<GameGamerReview> GameGamerReviews { get; set; }
}
