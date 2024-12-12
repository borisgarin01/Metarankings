using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public sealed record Critic
{
    public long Id { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Name { get; set; }

    [MaxLength(511)]
    [MinLength(1)]
    [Required]
    public string Url { get; set; }
    public IEnumerable<GameCriticReview> CriticReviews { get; set; }
}
