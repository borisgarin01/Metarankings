using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;

namespace Data.Repositories.Interfaces.Derived;

public interface IGamesPlayersReviewsRepository : IRepository<GameReview, AddGamePlayerReviewWithUserIdAndDateModel, UpdateGamePlayerReviewModel>
{
    public Task<GameReview> GetUserReviewForGameAsync(long userId, long gameId);
    public Task<IEnumerable<GameReview>> GetGameReviewsAsync(long gameId);
    public Task<IEnumerable<GameReview>> GetUserReviewsAsync(long userId);
}
