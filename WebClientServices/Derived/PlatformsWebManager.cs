using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class PlatformsWebManager : WebManager, IWebManager<Platform, AddPlatformModel, UpdatePlatformModel>
{
    public PlatformsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddPlatformModel addPlatformModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Platforms", addPlatformModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Platforms/platforms-excel-upload", formFile);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddPlatformModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Platforms/{id}");
        return httpResponseMessage;
    }

    public Task<IEnumerable<Platform>> GetAllAsync()
    {
        var platforms = HttpClient.GetFromJsonAsync<IEnumerable<Platform>>("/api/Platforms");
        return platforms;
    }

    public Task<IEnumerable<Platform>> GetAllAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<Platform> GetAsync(long id)
    {
        var platform = HttpClient.GetFromJsonAsync<Platform>($"/api/Platforms/{id}");
        return platform;
    }

    public async Task<Platform> UpdateAsync(long id, UpdatePlatformModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"/api/Platforms/{id}", tUpdate);
        var platform = await JsonSerializer.DeserializeAsync<Platform>(await httpResponseMessage.Content.ReadAsStreamAsync());
        return platform;
    }
}
