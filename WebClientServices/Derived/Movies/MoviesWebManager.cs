using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies.Movies;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesWebManager : WebManager, IWebManager<Movie, AddMovieModel, UpdateMovieModel>
{
    public MoviesWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieModel addMovieModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddMovieModel>("/api/movies/movies", addMovieModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieModel> addMoviesModels)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        var movies = await HttpClient.GetFromJsonAsync<IEnumerable<Movie>>("/api/Movies/Movies");
        return movies;
    }

    public async Task<IEnumerable<Movie>> GetFirstAsync(long offset, long limit)
    {
        var movies = await HttpClient.GetFromJsonAsync<IEnumerable<Movie>>($"/api/Movies/Movies/{offset}/{limit}");
        return movies;
    }

    public async Task<Movie> GetAsync(long id)
    {
        var movie = await HttpClient.GetFromJsonAsync<Movie>($"/api/Movies/Movies/{id}");
        return movie;
    }

    public async Task<Movie> UpdateAsync(long id, UpdateMovieModel updateMovieModel)
    {
        HttpResponseMessage publisherUpdateHttpResponseMessage = await HttpClient.PutAsJsonAsync($"/api/Movies/Movies/{id}", updateMovieModel);
        if (publisherUpdateHttpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<Movie>(await publisherUpdateHttpResponseMessage.Content.ReadAsStreamAsync());
        return null;
    }

    public Task<IEnumerable<Game>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
