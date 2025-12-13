using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesStudiosWebManager : WebManager, IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    public MoviesStudiosWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieStudioModel addMovieStudioModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Movies/MoviesStudios", addMovieStudioModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieStudioModel> addMoviesStudiosModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Movies/MoviesStudios", addMoviesStudiosModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Movies/MoviesStudios/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<MovieStudio>> GetAllAsync()
    {
        var moviesStudios = await HttpClient.GetFromJsonAsync<IEnumerable<MovieStudio>>($"/api/Movies/MoviesStudios");
        return moviesStudios;
    }

    public async Task<IEnumerable<MovieStudio>> GetFirstAsync(long offset, long limit)
    {
        var moviesStudios = await HttpClient.GetFromJsonAsync<IEnumerable<MovieStudio>>($"/api/Movies/MoviesStudios/{offset}/{limit}");
        return moviesStudios;
    }

    public async Task<MovieStudio> GetAsync(long id)
    {
        var moviesStudio = await HttpClient.GetFromJsonAsync<MovieStudio>($"/api/Movies/MoviesStudios/{id}");
        return moviesStudio;
    }

    public async Task<MovieStudio> UpdateAsync(long id, UpdateMovieStudioModel updateMovieStudioModel)
    {
        HttpResponseMessage updatingMovieStudioHttpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateMovieStudioModel>($"/api/Movies/MoviesStudios/{id}", updateMovieStudioModel);

        if (updatingMovieStudioHttpResponseMessage.IsSuccessStatusCode)
        {
            var movieStudio = await JsonSerializer.DeserializeAsync<MovieStudio>(await updatingMovieStudioHttpResponseMessage.Content.ReadAsStreamAsync());

            return movieStudio;
        }

        return null;
    }

    public Task<IEnumerable<Game>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
