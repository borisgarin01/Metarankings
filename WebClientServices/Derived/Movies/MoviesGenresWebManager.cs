using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesGenresWebManager : WebManager, IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public MoviesGenresWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieGenreModel addMovieGenreModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddMovieGenreModel>("/api/movies/moviesGenres", addMovieGenreModel);
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
        return await HttpClient.DeleteAsync($"/api/movies/moviesGenres/{id}");
    }

    public async Task<IEnumerable<MovieGenre>> GetAllAsync()
    {
        IEnumerable<MovieGenre> moviesGenres = await HttpClient.GetFromJsonAsync<IEnumerable<MovieGenre>>("/api/movies/moviesGenres");
        return moviesGenres;
    }

    public async Task<IEnumerable<MovieGenre>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<MovieGenre> moviesGenres = await HttpClient.GetFromJsonAsync<IEnumerable<MovieGenre>>($"/api/movies/moviesGenres/{offset}/{limit}");
        return moviesGenres;
    }

    public async Task<MovieGenre> GetAsync(long id)
    {
        MovieGenre movieGenre = await HttpClient.GetFromJsonAsync<MovieGenre>($"/api/Movies/MoviesGenres/{id}");
        return movieGenre;
    }

    public async Task<MovieGenre> UpdateAsync(long id, UpdateMovieGenreModel updateMovieGenreModel)
    {
        var httpResponseMessage = await HttpClient.PutAsJsonAsync($"/api/Movies/MoviesGenres/{id}", updateMovieGenreModel);

        if (httpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<MovieGenre>(await httpResponseMessage.Content.ReadAsStreamAsync());

        return null;
    }

    public Task<IEnumerable<Game>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
