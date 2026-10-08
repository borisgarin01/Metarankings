using Data.Repositories.Classes;
using Domain.Movies;
using Domain.RequestsModels.Movies;
using Domain.RequestsModels.Movies.Movies;

namespace Data.Repositories.Interfaces.Derived;

public interface IMoviesRepository : IRepository<Movie, AddMovieModel, UpdateMovieModel>
{
    public Task<IEnumerable<Movie>> GetAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetByGenreAsync(long genreId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetByParametersAsync(MovieFilterRequest filter, CancellationToken cancellationToken = default);

    Task<int> GetCountByParametersAsync(MovieFilterRequest filter, CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetMostWaitingAsync(long[]? genresIds, int skip, int take, CancellationToken cancellationToken = default);

    Task<int> GetMostWaitingCountAsync(long[]? genresIds, CancellationToken cancellationToken = default);
}
