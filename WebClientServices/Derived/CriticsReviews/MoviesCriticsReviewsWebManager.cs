namespace WebManagers.Derived.CriticsReviews;

public sealed class MoviesCriticsReviewsWebManager : CriticsReviewsWebManager
{
    public MoviesCriticsReviewsWebManager(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory, "/api/movies/MoviesCriticsReviews")
    {
    }
}
