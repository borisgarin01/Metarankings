using System.Text.Json.Serialization;

namespace Domain;

public sealed record GameImage
{
    public long Id { get; set; }
    [JsonIgnore]
    public Game Game { get; set; }
    public long GameId { get; set; }
    public required string Href { get; set; }
}
