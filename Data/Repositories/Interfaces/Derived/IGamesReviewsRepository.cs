using Domain.Games;
using Domain.Reviews;

namespace Data.Repositories.Interfaces.Derived;

public interface IGamesReviewsRepository : IRepository<GameReview>
{
    public Task<GameReview> GetUserReviewForGameAsync(long userId, long gameId);
    public Task<IEnumerable<GameReview>> GetGameReviewsAsync(long gameId);
    public Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId);
}
