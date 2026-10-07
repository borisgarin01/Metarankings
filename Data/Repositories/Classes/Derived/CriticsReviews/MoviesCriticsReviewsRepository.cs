namespace Data.Repositories.Classes.Derived.CriticsReviews;

public sealed class MoviesCriticsReviewsRepository : CriticsReviewsRepository
{
    public MoviesCriticsReviewsRepository(string connectionString)
        : base(connectionString, "MoviesCriticsReviews", "MovieId")
    {
    }
}
