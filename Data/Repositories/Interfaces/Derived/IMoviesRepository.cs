using Data.Repositories.Classes.Derived.Movies;
using Domain.Movies;

namespace Data.Repositories.Interfaces.Derived;

public interface IMoviesRepository : IRepository<Movie, AddMovieModel, UpdateMovieModel>
{
    public Task<IEnumerable<Movie>> GetAsync(DateTime dateFrom, DateTime dateTo);
}
