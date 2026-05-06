using Domain.Games;
using Domain.Games.Collections;
using Domain.RequestsModels.Games.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesCollectionsItemsWebManager : WebManager, IWebManager<GamesCollectionItem, AddGamesCollectionItemModel, UpdateGamesCollectionItemModel>
{
    public GamesCollectionsItemsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGamesCollectionItemModel addGamesCollectionItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Games/CollectionsItems", addGamesCollectionItemModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGamesCollectionItemModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Games/CollectionsItems/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<GamesCollectionItem>> GetAllAsync()
    {
        var gamesCollectionsItems = await HttpClient.GetFromJsonAsync<IEnumerable<GamesCollectionItem>>($"/api/Games/CollectionsItems");
        return gamesCollectionsItems;
    }

    public async Task<GamesCollectionItem> GetAsync(long id)
    {
        var gamesCollectionItem = await HttpClient.GetFromJsonAsync<GamesCollectionItem>($"/api/Games/CollectionsItems/{id}");
        return gamesCollectionItem;
    }

    public async Task<IEnumerable<GamesCollectionItem>> GetFirstAsync(long offset, long limit)
    {
        var gamesCollectionsItems = await HttpClient.GetFromJsonAsync<IEnumerable<GamesCollectionItem>>($"/api/Games/CollectionsItems/{offset}/{limit}");
        return gamesCollectionsItems;
    }

    public Task<IEnumerable<GamesCollectionItem>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task<GamesCollectionItem> UpdateAsync(long id, UpdateGamesCollectionItemModel updateGamesCollectionItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateGamesCollectionItemModel>($"/api/Games/CollectionsItems/{id}", updateGamesCollectionItemModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            var updatedGameCollectionItem = await JsonSerializer.DeserializeAsync<GamesCollectionItem>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedGameCollectionItem;
        }

        return null;
    }
}
