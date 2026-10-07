namespace Data.Repositories.Classes.Derived.CriticsReviews;

public sealed class GamesCriticsReviewsRepository : CriticsReviewsRepository
{
    public GamesCriticsReviewsRepository(string connectionString)
        : base(connectionString, "GamesCriticsReviews", "GameId")
    {
    }
}
