using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class PlatformsWebManager : WebManager, IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>
{
    public PlatformsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddPlatformModel addPlatformModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Platforms", addPlatformModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Platforms/platforms-excel-upload", formFile);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddPlatformModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Games/Platforms/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Platform>> GetAllAsync()
    {
        IEnumerable<Platform>? platforms = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Platform>>("/api/Games/Platforms");
        return platforms;
    }

    public async Task<IEnumerable<Platform>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Platform>? platforms = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Platform>>($"/api/Games/Platforms/{offset}/{limit}");
        return platforms;
    }

    public async Task<Platform> GetAsync(long id)
    {
        Platform? platform = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Platform>($"/api/Games/Platforms/{id}");
        return platform;
    }

    public async Task<Platform> UpdateAsync(long id, UpdatePlatformModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Games/Platforms/{id}", tUpdate);
        Platform? platform = await JsonSerializer.DeserializeAsync<Platform>(await httpResponseMessage.Content.ReadAsStreamAsync());
        return platform;
    }

    public Task<IEnumerable<Platform>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
