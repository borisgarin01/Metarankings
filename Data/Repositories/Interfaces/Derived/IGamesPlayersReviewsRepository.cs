using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;

namespace Data.Repositories.Interfaces.Derived;

public interface IGamesPlayersReviewsRepository : IRepository<GameReview, AddGamePlayerReviewWithUserIdAndDateModel, UpdateGamePlayerReviewModel>
{
    public Task<GameReview> GetUserReviewForGameAsync(long userId, long gameId, CancellationToken cancellationToken = default);
    public Task<IEnumerable<GameReview>> GetGameReviewsAsync(long gameId, CancellationToken cancellationToken = default);
    public Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId, CancellationToken cancellationToken = default);
}
