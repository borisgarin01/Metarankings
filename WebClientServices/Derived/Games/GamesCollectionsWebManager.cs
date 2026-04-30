using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesCollectionsWebManager : WebManager, IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    public GamesCollectionsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGamesCollectionModel addGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Games/Collections", addGameCollectionModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGamesCollectionModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Games/Collections/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<GamesCollection>> GetAllAsync()
    {
        var gamesCollections = await HttpClient.GetFromJsonAsync<IEnumerable<GamesCollection>>($"/api/Games/Collections");
        return gamesCollections;
    }

    public async Task<IEnumerable<GamesCollection>> GetFirstAsync(long offset, long limit)
    {
        var gameCollections = await HttpClient.GetFromJsonAsync<IEnumerable<GamesCollection>>($"/api/Games/Collections/{offset}/{limit}");
        return gameCollections;
    }

    public async Task<GamesCollection> GetAsync(long id)
    {
        var gameCollection = await HttpClient.GetFromJsonAsync<GamesCollection>($"/api/Games/Collections/{id}");
        return gameCollection;
    }

    public async Task<GamesCollection> UpdateAsync(long id, UpdateGamesCollectionModel updateGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateGamesCollectionModel>($"/api/Games/Collections/{id}", updateGameCollectionModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            var updatedGameCollection = await JsonSerializer.DeserializeAsync<GamesCollection>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedGameCollection;
        }

        return null;
    }

    public Task<IEnumerable<Game>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
