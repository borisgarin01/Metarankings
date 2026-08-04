using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class DevelopersWebManager : WebManager, IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>
{
    public DevelopersWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddDeveloperModel addDeveloperModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Developers", addDeveloperModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Developers/developers-excel-upload", formFile);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddDeveloperModel> addDevelopersModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Developers/upload-developers-from-json", addDevelopersModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Games/Developers/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Developer>> GetAllAsync()
    {
        IEnumerable<Developer>? developers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Developer>>($"/api/Games/Developers");
        return developers;
    }

    public async Task<IEnumerable<Developer>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Developer>? developers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Developer>>($"/api/Games/Developers/{offset}/{limit}");
        return developers;
    }

    public async Task<Developer> GetAsync(long id)
    {
        Developer? developer = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Developer>($"/api/Games/Developers/{id}");
        return developer;
    }

    public async Task<Developer> UpdateAsync(long id, UpdateDeveloperModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Games/Developers/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Developer>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Developer>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
