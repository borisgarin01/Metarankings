using Domain.Games;
using Domain.RequestsModels.Games;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class GamesWebManager : WebManager, IWebManager<Game, AddGameModel, UpdateGameModel>
{
    public GamesWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGameModel addGameModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddGameModel>("/api/Games/Games", addGameModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameModel> addGamesModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/games/Games/upload-games-from-json", addGamesModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/games/Games/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        var games = await HttpClient.GetFromJsonAsync<IEnumerable<Game>>("/api/Games/Games");
        return games;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(long offset, long limit)
    {
        var games = await HttpClient.GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/{offset}/{limit}");
        return games;
    }

    public async Task<Game> GetAsync(long id)
    {
        var game = await HttpClient.GetFromJsonAsync<Game>($"/api/Games/Games/{id}");
        return game;
    }

    public async Task<Game> UpdateAsync(long id, UpdateGameModel updateGameModel)
    {
        HttpResponseMessage publisherUpdateHttpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateGameModel>($"/api/Games/Games/{id}", updateGameModel);
        if (publisherUpdateHttpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<Game>(await publisherUpdateHttpResponseMessage.Content.ReadAsStreamAsync());
        return null;
    }
}
