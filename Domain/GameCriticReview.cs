namespace Domain;
public sealed record GameCriticReview
{
    public long Id { get; set; }
    public Critic Critic { get; set; }
    public long CriticId { get; set; }
    public Game Game { get; set; }
    public long GameId { get; set; }
    public string Text { get; set; }
    public float Score { get; set; }
    public string Url { get; set; }
    public DateTime ReviewDate { get; set; }
}
