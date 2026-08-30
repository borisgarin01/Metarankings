using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesGenresWebManager : WebManager, IWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public MoviesGenresWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieGenreModel addMovieGenreModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync<AddMovieGenreModel>("/api/movies/genres", addMovieGenreModel);
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

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        return await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/movies/genres/{id}");
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        IEnumerable<Genre> moviesGenres = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Genre>>("/api/movies/genres");
        return moviesGenres;
    }

    public async Task<IEnumerable<Genre>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Genre> moviesGenres = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Genre>>($"/api/movies/genres/{offset}/{limit}");
        return moviesGenres;
    }

    public async Task<Genre> GetAsync(long id)
    {
        Genre movieGenre = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Genre>($"/api/Movies/genres/{id}");
        return movieGenre;
    }

    public async Task<Genre> UpdateAsync(long id, UpdateMovieGenreModel updateMovieGenreModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Movies/genres/{id}", updateMovieGenreModel);

        if (httpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<Genre>(await httpResponseMessage.Content.ReadAsStreamAsync());

        return null;
    }

    public Task<IEnumerable<Genre>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
