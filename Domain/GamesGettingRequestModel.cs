namespace Domain;

public sealed record GamesGettingRequestModel
{
    public int[] ReleasesYears { get; set; } = Array.Empty<int>();
    public int[] DevelopersIds { get; set; } = Array.Empty<int>();
    public int[] GenresIds { get; set; } = Array.Empty<int>();
    public int[] PublishersIds { get; set; } = Array.Empty<int>();
    public int[] PlatformsIds { get; set; } = Array.Empty<int>();
}
