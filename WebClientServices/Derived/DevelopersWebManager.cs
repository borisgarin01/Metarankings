using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class DevelopersWebManager : WebManager, IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>
{
    public DevelopersWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddDeveloperModel addDeveloperModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Developers", addDeveloperModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Developers/developers-excel-upload", formFile);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddDeveloperModel> addDevelopersModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Developers/upload-developers-from-json", addDevelopersModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Developers/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Developer>> GetAllAsync()
    {
        var developers = await HttpClient.GetFromJsonAsync<IEnumerable<Developer>>($"/api/Developers");
        return developers;
    }

    public async Task<IEnumerable<Developer>> GetAllAsync(long offset, long limit)
    {
        var developers = await HttpClient.GetFromJsonAsync<IEnumerable<Developer>>($"/api/Developers/{offset}/{limit}");
        return developers;
    }

    public async Task<Developer> GetAsync(long id)
    {
        var developer = await HttpClient.GetFromJsonAsync<Developer>($"/api/Developers/{id}");
        return developer;
    }

    public async Task<Developer> UpdateAsync(long id, UpdateDeveloperModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"/api/Developers/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Developer>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }
}
