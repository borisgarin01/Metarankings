using Microsoft.EntityFrameworkCore;

namespace Domain;

[Index(nameof(Text), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public sealed record GameGamerReview
{
    public long Id { get; set; }
    public Gamer Gamer { get; set; }
    public long GamerId { get; set; }
    public Game Game { get; set; }
    public long GameId { get; set; }
    public string Text { get; set; }
    public float Score { get; set; }
    public string Url { get; set; }
    public DateTime ReviewDate { get; set; }
}
