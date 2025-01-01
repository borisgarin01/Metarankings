using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain;

[Index(nameof(Url), IsUnique = true)]
public sealed record Trailer
{
    public long Id { get; set; }

    [Required]
    public string Url { get; set; }

    [JsonIgnore]
    public Game Game { get; set; }
    public long GameId { get; set; }
}
