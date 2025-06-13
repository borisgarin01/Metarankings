namespace Domain.Games;

public sealed record GameRow
{
    public long GameId { get; set; }
    public string GameName { get; set; }
    public string GameImage { get; set; }
    public DateOnly GameReleaseDate { get; set; }
    public string GameDescription { get; set; }
    public long DeveloperId { get; set; }
    public string DeveloperName { get; set; }
    public long PublisherId { get; set; }
    public string PublisherName { get; set; }
    public long GenreId { get; set; }
    public string GenreName { get; set; }
    public long LocalizationId { get; set; }
    public string LocalizationName { get; set; }
    public long PlatformId { get; set; }
    public string PlatformName { get; set; }
    public long GameScreenshotId { get; set; }
    public long GameScreenshotGameId { get; set; }
    public string GameTrailer { get; set; }
}
