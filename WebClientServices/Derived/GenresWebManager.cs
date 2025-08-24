using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class GenresWebManager : WebManager, IWebManager<Genre, AddGenreModel, UpdateGenreModel>
{
    public GenresWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGenreModel addGenreModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Genres", addGenreModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Genres/genres-excel-upload", formFile);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGenreModel> addGenresModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Genres/upload-genres-from-json", addGenresModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Genres/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        var genres = await HttpClient.GetFromJsonAsync<IEnumerable<Genre>>($"/api/Genres");
        return genres;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync(long offset, long limit)
    {
        var genres = await HttpClient.GetFromJsonAsync<IEnumerable<Genre>>($"/api/Genres/{offset}/{limit}");
        return genres;
    }

    public async Task<Genre> GetAsync(long id)
    {
        var genre = await HttpClient.GetFromJsonAsync<Genre>($"/api/Genres/{id}");
        return genre;
    }

    public async Task<Genre> UpdateAsync(long id, UpdateGenreModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"/api/Genres/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Genre>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }
}