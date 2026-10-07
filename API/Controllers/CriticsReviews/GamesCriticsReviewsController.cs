using Data.Repositories.Classes.Derived.CriticsReviews;

namespace API.Controllers.CriticsReviews;

[Route("api/games/[controller]")]
public sealed class GamesCriticsReviewsController : CriticsReviewsControllerBase
{
    public GamesCriticsReviewsController(GamesCriticsReviewsRepository gamesCriticsReviewsRepository, ILogger<GamesCriticsReviewsController> logger)
        : base(gamesCriticsReviewsRepository, logger)
    {
    }
}
