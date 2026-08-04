using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesStudiosWebManager : WebManager, IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    public MoviesStudiosWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieStudioModel addMovieStudioModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Movies/MoviesStudios", addMovieStudioModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieStudioModel> addMoviesStudiosModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Movies/MoviesStudios", addMoviesStudiosModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Movies/MoviesStudios/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<MovieStudio>> GetAllAsync()
    {
        IEnumerable<MovieStudio>? moviesStudios = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<MovieStudio>>($"/api/Movies/MoviesStudios");
        return moviesStudios;
    }

    public async Task<IEnumerable<MovieStudio>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<MovieStudio>? moviesStudios = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<MovieStudio>>($"/api/Movies/MoviesStudios/{offset}/{limit}");
        return moviesStudios;
    }

    public async Task<MovieStudio> GetAsync(long id)
    {
        MovieStudio? moviesStudio = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<MovieStudio>($"/api/Movies/MoviesStudios/{id}");
        return moviesStudio;
    }

    public async Task<MovieStudio> UpdateAsync(long id, UpdateMovieStudioModel updateMovieStudioModel)
    {
        HttpResponseMessage updatingMovieStudioHttpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync<UpdateMovieStudioModel>($"/api/Movies/MoviesStudios/{id}", updateMovieStudioModel);

        if (updatingMovieStudioHttpResponseMessage.IsSuccessStatusCode)
        {
            MovieStudio? movieStudio = await JsonSerializer.DeserializeAsync<MovieStudio>(await updatingMovieStudioHttpResponseMessage.Content.ReadAsStreamAsync());

            return movieStudio;
        }

        return null;
    }

    public Task<IEnumerable<MovieStudio>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
