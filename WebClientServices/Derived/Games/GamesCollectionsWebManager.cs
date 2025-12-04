using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesCollectionsWebManager : WebManager, IWebManager<GameCollection, AddGameCollectionModel, UpdateGameCollectionModel>
{
    public GamesCollectionsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGameCollectionModel addGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Games/Collections", addGameCollectionModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameCollectionModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Games/Collections/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<GameCollection>> GetAllAsync()
    {
        var gamesCollections = await HttpClient.GetFromJsonAsync<IEnumerable<GameCollection>>($"/api/Games/Collections");
        return gamesCollections;
    }

    public async Task<IEnumerable<GameCollection>> GetAllAsync(long offset, long limit)
    {
        var gameCollections = await HttpClient.GetFromJsonAsync<IEnumerable<GameCollection>>($"/api/Games/Collections/{offset}/{limit}");
        return gameCollections;
    }

    public async Task<GameCollection> GetAsync(long id)
    {
        var gameCollection = await HttpClient.GetFromJsonAsync<GameCollection>($"/api/Games/Collections/{id}");
        return gameCollection;
    }

    public async Task<GameCollection> UpdateAsync(long id, UpdateGameCollectionModel updateGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateGameCollectionModel>($"/api/Games/Collections/{id}", updateGameCollectionModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            var updatedGameCollection = await JsonSerializer.DeserializeAsync<GameCollection>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedGameCollection;
        }

        return null;
    }
}
