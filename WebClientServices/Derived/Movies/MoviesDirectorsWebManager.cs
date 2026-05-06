using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

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

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"api/movies/moviesDirectors/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<MovieDirector>> GetAllAsync()
    {
        var moviesDirectors = await HttpClient.GetFromJsonAsync<IEnumerable<MovieDirector>>("api/movies/moviesDirectors");
        return moviesDirectors;
    }

    public async Task<IEnumerable<MovieDirector>> GetFirstAsync(long offset, long limit)
    {
        var moviesDirectors = await HttpClient.GetFromJsonAsync<IEnumerable<MovieDirector>>($"api/movies/moviesDirectors/{offset}/{limit}");
        return moviesDirectors;
    }

    public async Task<MovieDirector> GetAsync(long id)
    {
        var movieDirector = await HttpClient.GetFromJsonAsync<MovieDirector>($"api/movies/moviesDirectors/{id}");
        return movieDirector;
    }

    public async Task<MovieDirector> UpdateAsync(long id, UpdateMovieDirectorModel updateMovieDirectorModel)
    {
        HttpResponseMessage updateMovieDirectorHttpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateMovieDirectorModel>($"api/movies/moviesDirectors/{id}", updateMovieDirectorModel);
        if (updateMovieDirectorHttpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<MovieDirector>(await updateMovieDirectorHttpResponseMessage.Content.ReadAsStreamAsync());
        return null;
    }

    public Task<IEnumerable<MovieDirector>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
