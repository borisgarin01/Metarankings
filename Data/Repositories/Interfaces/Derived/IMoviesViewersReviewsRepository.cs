using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;

namespace Data.Repositories.Interfaces.Derived;

public interface IMoviesViewersReviewsRepository : IRepository<MovieViewerReview, AddMovieViewerReviewWithUserIdAndDateModel, UpdateMovieViewerReviewModel>
{
    public Task<IEnumerable<MovieViewerReview>> GetByTimespanAsync(DateTime dateFrom, DateTime dateTo);
    public Task<MovieViewerReview> GetUserReviewForMovieAsync(long userId, long movieId);
}
