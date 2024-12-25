using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Domain;

[Index(nameof(GameId), IsUnique = true)]
[Index(nameof(GameGenreId), IsUnique = true)]
public sealed record GameGameGenre
{
    public long Id { get; set; }
    public long GameId { get; set; }

    [JsonIgnore]
    public Game Game { get; set; }
    public long GameGenreId { get; set; }

    [JsonIgnore]
    public GameGenre GameGenre { get; set; }
}
