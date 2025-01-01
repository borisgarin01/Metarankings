using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain;

[Index(nameof(Text), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public sealed record GameCriticReview
{
    public long Id { get; set; }
    public Critic Critic { get; set; }
    public long CriticId { get; set; }

    [JsonIgnore]
    public Game Game { get; set; }
    public long GameId { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public float Score { get; set; }

    [Required]
    [MaxLength(511)]
    [MinLength(1)]
    public string Url { get; set; }

    [Required]
    public DateTime? ReviewDate { get; set; } = DateTime.UtcNow;
}
