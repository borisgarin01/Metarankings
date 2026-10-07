using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Games;

public sealed class DevelopersWebManager : CrudWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel>
{
    public DevelopersWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Developers")
    {
    }

    public override Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        return PostAsync("developers-excel-upload", formFile);
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddDeveloperModel> addDevelopersModels)
    {
        return PostAsync("upload-developers-from-json", addDevelopersModels);
    }
}
