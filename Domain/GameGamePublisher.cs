using Microsoft.EntityFrameworkCore;

namespace Domain;

[Index(nameof(GameId), nameof(GamePublisherId), IsUnique = true)]
public sealed record GameGamePublisher
{
    public long Id { get; set; }
    public Game Game { get; set; }
    public long GameId { get; set; }
    public GamePublisher GamePublisher { get; set; }
    public long GamePublisherId { get; set; }
}

