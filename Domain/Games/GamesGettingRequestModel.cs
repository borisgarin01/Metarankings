namespace Domain.Games;

public sealed record GamesGettingRequestModel(
    int[]? ReleasesYears,
    int[]? DevelopersIds,
    int[]? GenresIds,
    int[]? PublishersIds,
    int[]? PlatformsIds
);
