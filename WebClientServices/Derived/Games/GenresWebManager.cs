using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class GenresWebManager : WebManager, IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel>
{
    public GenresWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGameGenreModel addGenreModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Genres", addGenreModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Genres/genres-excel-upload", formFile);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameGenreModel> addGenresModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Genres/upload-genres-from-json", addGenresModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Games/Genres/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        IEnumerable<Genre>? genres = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Genre>>($"/api/Games/Genres");
        return genres;
    }

    public async Task<IEnumerable<Genre>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Genre>? genres = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Genre>>($"/api/Games/Genres/{offset}/{limit}");
        return genres;
    }

    public async Task<Genre> GetAsync(long id)
    {
        Genre? genre = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Genre>($"/api/Games/Genres/{id}");
        return genre;
    }

    public async Task<Genre> UpdateAsync(long id, UpdateGameGenreModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Games/Genres/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Genre>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Genre>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}