using Data.Repositories.Classes.Derived.CriticsReviews;

namespace API.Controllers.CriticsReviews;

[Route("api/movies/[controller]")]
public sealed class MoviesCriticsReviewsController : CriticsReviewsControllerBase
{
    public MoviesCriticsReviewsController(MoviesCriticsReviewsRepository moviesCriticsReviewsRepository, ILogger<MoviesCriticsReviewsController> logger)
        : base(moviesCriticsReviewsRepository, logger)
    {
    }
}
