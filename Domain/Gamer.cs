using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Url), IsUnique = true)]
[Index(nameof(AccountName), IsUnique = true)]
public sealed record Gamer
{
    public long Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Url { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string AccountName { get; set; }
    public IEnumerable<GameGamerReview> GameGamerReviews { get; set; }
}
