namespace Domain.RequestsModels;

public sealed record GameFilterRequest
{
    public long[] GenresIds { get; set; } = Array.Empty<long>();
    public long[] PlatformsIds { get; set; } = Array.Empty<long>();
    public long[] Years { get; set; } = Array.Empty<long>();
    public long[] DevelopersIds { get; set; } = Array.Empty<long>();
    public long[] PublishersIds { get; set; } = Array.Empty<long>();
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
}
