namespace Domain;

public sealed record GamesGettingRequestModel
{
    public int[]? ReleasesYears { get; set; }
    public int[]? DevelopersIds { get; set; }
    public int[]? GenresIds { get; set; }
    public int[]? PublishersIds { get; set; }
    public int[]? PlatformsIds { get; set; }
}
