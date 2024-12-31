using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Domain;

[Index(nameof(GameId), nameof(GamePlatformId), IsUnique = true)]
public sealed record GameGamePlatform
{
    public long Id { get; set; }
    public long GameId { get; set; }

    [JsonIgnore]
    public Game Game { get; set; }
    public long GamePlatformId { get; set; }

    [JsonIgnore]
    public GamePlatform GamePlatform { get; set; }
}
