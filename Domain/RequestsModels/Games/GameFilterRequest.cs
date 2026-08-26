namespace Domain.RequestsModels.Games;

public sealed record GameFilterRequest
{
    public long[]? GenresIds { get; set; }
    public long[]? PlatformsIds { get; set; }
    public int[]? Years { get; set; }
    public long[]? DevelopersIds { get; set; }
    public long[]? PublishersIds { get; set; }
    public long[]? LocalizationIds { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
}
