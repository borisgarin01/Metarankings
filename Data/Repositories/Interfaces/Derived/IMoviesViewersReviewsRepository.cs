using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;

namespace Data.Repositories.Interfaces.Derived;

public interface IMoviesViewersReviewsRepository : IRepository<MovieReview, AddMovieViewerReviewWithUserIdAndDateModel, UpdateMovieViewerReviewWithUserIdAndDateModel>
{
    public Task<IEnumerable<MovieReview>> GetByTimespanAsync(DateTime dateFrom, DateTime dateTo);
    public Task<MovieReview> GetUserReviewForMovieAsync(long userId, long movieId);
}
