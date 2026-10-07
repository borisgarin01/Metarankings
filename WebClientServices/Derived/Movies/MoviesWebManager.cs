using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesWebManager : WebManager, IWebManager<Movie, AddMovieModel, UpdateMovieModel>, IByNameSearchingManager<Movie>
{
    public MoviesWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieModel addMovieModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync<AddMovieModel>("/api/movies", addMovieModel);
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

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/movies/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        IEnumerable<Movie>? movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>("/api/movies");
        return movies;
    }

    public async Task<IEnumerable<Movie>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Movie>? movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>($"/api/movies/{offset}/{limit}");
        return movies;
    }

    public async Task<Movie> GetAsync(long id)
    {
        Movie? movie = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Movie>($"/api/movies/{id}");
        return movie;
    }

    public async Task<Movie> UpdateAsync(long id, UpdateMovieModel updateMovieModel)
    {
        HttpResponseMessage publisherUpdateHttpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/movies/{id}", updateMovieModel);
        if (publisherUpdateHttpResponseMessage.IsSuccessStatusCode)
            return await publisherUpdateHttpResponseMessage.Content.ReadFromJsonAsync<Movie>();
        return null;
    }

    public Task<IEnumerable<Movie>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Movie>> GetMostWaitingAsync(
        int offset,
        int limit,
        IEnumerable<long>? genresIds = null)
    {
        IEnumerable<Movie>? movies = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<IEnumerable<Movie>>(
                $"/api/movies/most-waiting/{offset}/{limit}{BuildGenresQuery(genresIds)}");

        return movies ?? Enumerable.Empty<Movie>();
    }

    public async Task<int> GetMostWaitingCountAsync(IEnumerable<long>? genresIds = null)
    {
        int count = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<int>($"/api/movies/most-waiting/count{BuildGenresQuery(genresIds)}");

        return count;
    }

    private static string BuildGenresQuery(IEnumerable<long>? genresIds)
    {
        return genresIds?.Any() == true
            ? "?" + string.Join("&", genresIds.Select(id => $"genresIds={id}"))
            : string.Empty;
    }

    public async Task<IEnumerable<Movie>> SearchByName(string name)
    {
        IEnumerable<Movie> movies;
        if (!string.IsNullOrWhiteSpace(name))
            movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>($"/api/movies/Search?name={name}");
        else
            movies = await GetAllAsync();
        return movies;
    }
}
