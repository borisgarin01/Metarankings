using Data.Repositories.Classes.Derived.CriticsReviews;

namespace API.Controllers.CriticsReviews;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Movies })]
[Route("api/movies/[controller]")]
public sealed class MoviesCriticsReviewsController : CriticsReviewsControllerBase
{
    public MoviesCriticsReviewsController(MoviesCriticsReviewsRepository moviesCriticsReviewsRepository, ILogger<MoviesCriticsReviewsController> logger)
        : base(moviesCriticsReviewsRepository, logger)
    {
    }
}
