using Domain.Games;
using Domain.RequestsModels.Games.Platforms;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Games;

public sealed class PlatformsWebManager : CrudWebManager<Platform, AddPlatformModel, UpdatePlatformModel>
{
    public PlatformsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Platforms")
    {
    }

    public override Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        return PostAsync("platforms-excel-upload", formFile, cancellationToken);
    }
}
