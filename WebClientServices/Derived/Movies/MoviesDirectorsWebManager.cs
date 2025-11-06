
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesDirectorsWebManager : WebManager, IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>
{
    public MoviesDirectorsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieDirectorModel addMovieDirectorModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddMovieDirectorModel>("api/movies/moviesDirectors", addMovieDirectorModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieDirectorModel> adds)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieDirector>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieDirector>> GetAllAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<MovieDirector> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<MovieDirector> UpdateAsync(long id, UpdateMovieDirectorModel tUpdate)
    {
        throw new NotImplementedException();
    }
}
