using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain;
public sealed record GameScreenshot
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [Required]
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [Required]
    [JsonPropertyName("gameId")]
    public decimal GameId { get; set; }
}
