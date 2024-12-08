namespace Domain;
public sealed record Game
{
    public long Id { get; set; }
    public string Name { get; set; }
    public float Score { get; set; }
    public string Description { get; set; }
    public GameLocalization Localization { get; set; }
    public long LocalizationId { get; set; }
    public IEnumerable<GameDeveloper> Developers { get; set; }
    public IEnumerable<GamePublisher> Publishers { get; set; }
    public IEnumerable<GamePlatform> Platforms { get; set; }
    public IEnumerable<GameGenre> Genres { get; set; }
    public DateTime ReleaseDate { get; set; }
    public IEnumerable<GameTag> Tags { get; set; }
    public IEnumerable<GameCriticReview> CriticsReviews { get; set; }
    public IEnumerable<GameGamerReview> UsersReviews { get; set; }
}
