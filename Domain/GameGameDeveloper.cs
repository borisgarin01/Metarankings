using Microsoft.EntityFrameworkCore;

namespace Domain;

[Index(nameof(GameId), nameof(GameDeveloperId), IsUnique = true)]
public sealed record GameGameDeveloper
{
    public long Id { get; set; }
    public Game Game { get; set; }
    public long GameId { get; set; }
    public GameDeveloper GameDeveloper { get; set; }
    public long GameDeveloperId { get; set; }
}
