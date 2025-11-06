using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Movies;

public sealed class MoviesGenresWebManager : IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public Task<HttpResponseMessage> AddAsync(AddMovieGenreModel tAdd)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieGenreModel> adds)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieGenre>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieGenre>> GetAllAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<MovieGenre> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<MovieGenre> UpdateAsync(long id, UpdateMovieGenreModel tUpdate)
    {
        throw new NotImplementedException();
    }
}
