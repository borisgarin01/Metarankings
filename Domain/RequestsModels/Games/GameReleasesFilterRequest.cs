namespace Domain.RequestsModels.Games;

public sealed record GameReleasesFilterRequest
{
    public long[]? GenresIds { get; set; }
    public long[]? PlatformsIds { get; set; }
    public short Offset { get; set; } = 0;
    public short Limit { get; set; } = 20;
}
