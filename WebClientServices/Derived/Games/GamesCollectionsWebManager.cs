using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesCollectionsWebManager : WebManager, IWebManager<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel>
{
    public GamesCollectionsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGamesCollectionModel addGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Collections", addGameCollectionModel);
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
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Games/Collections/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<GamesCollection>> GetAllAsync()
    {
        IEnumerable<GamesCollection>? gamesCollections = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<GamesCollection>>($"/api/Games/Collections");
        return gamesCollections;
    }

    public async Task<IEnumerable<GamesCollection>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<GamesCollection>? gameCollections = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<GamesCollection>>($"/api/Games/Collections/{offset}/{limit}");
        return gameCollections;
    }

    public async Task<GamesCollection> GetAsync(long id)
    {
        GamesCollection? gameCollection = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<GamesCollection>($"/api/Games/Collections/{id}");
        return gameCollection;
    }

    public async Task<GamesCollection> UpdateAsync(long id, UpdateGamesCollectionModel updateGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync<UpdateGamesCollectionModel>($"/api/Games/Collections/{id}", updateGameCollectionModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            GamesCollection? updatedGameCollection = await JsonSerializer.DeserializeAsync<GamesCollection>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedGameCollection;
        }

        return null;
    }

    public Task<IEnumerable<GamesCollection>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
