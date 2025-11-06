using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesGenresWebManager : WebManager, IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public MoviesGenresWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieGenreModel addMovieGenreModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddMovieGenreModel>("api/movies/moviesGenres", addMovieGenreModel);
        return httpResponseMessage;
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
