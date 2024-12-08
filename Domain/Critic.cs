namespace Domain;
public sealed record Critic
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public IEnumerable<GameCriticReview> CriticReviews { get; set; }
}
