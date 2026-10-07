namespace WebManagers.Derived.CriticsReviews;

public sealed class GamesCriticsReviewsWebManager : CriticsReviewsWebManager
{
    public GamesCriticsReviewsWebManager(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory, "/api/games/GamesCriticsReviews")
    {
    }
}
