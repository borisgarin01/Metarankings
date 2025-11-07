using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Movies;

public sealed class MoviesWebManager : WebManager, IWebManager<Movie, AddMovieModel, UpdateMovieModel>
{
    public MoviesWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public Task<HttpResponseMessage> AddAsync(AddMovieModel addMovieModel)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieModel> addMoviesModels)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetAllAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<Movie> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Movie> UpdateAsync(long id, UpdateMovieModel updateMovieModel)
    {
        throw new NotImplementedException();
    }
}
