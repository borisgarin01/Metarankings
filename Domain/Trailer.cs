using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Url), IsUnique = true)]
public sealed record Trailer
{
    public long Id { get; set; }

    [Required]
    public string Url { get; set; }
    public Game Game { get; set; }
    public long GameId { get; set; }
}
