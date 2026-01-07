using Data.Repositories.Classes;
using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;

namespace Data.Repositories.Interfaces.Derived;

public interface IMoviesRepository : IRepository<Movie, AddMovieModel, UpdateMovieModel>
{
    public Task<IEnumerable<Movie>> GetAsync(DateTime dateFrom, DateTime dateTo);
}
