using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using WebManagers;

namespace WebClientServices;

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

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddDeveloperModel> adds)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Developers/upload-developers-from-json", adds);
        return httpResponseMessage;
    }

    public Task<IEnumerable<Developer>> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Developer>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Developer>> GetAllAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Developer>> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Developer>> UpdateAsync(long id, UpdateDeveloperModel tUpdate)
    {
        throw new NotImplementedException();
    }
}
