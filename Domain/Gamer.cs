using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Url), IsUnique = true)]
[Index(nameof(AccountName), IsUnique = true)]
public record Gamer
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
    public virtual IEnumerable<GameGamerReview> GameGamerReviews { get; set; }
}
