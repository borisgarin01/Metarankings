using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Games;

public sealed class LocalizationsWebManager : CrudWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    public LocalizationsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Localizations")
    {
    }

    public override Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile, CancellationToken cancellationToken = default)
    {
        return PostAsync("localizations-excel-upload", formFile, cancellationToken);
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddLocalizationModel> addLocalizationsModels, CancellationToken cancellationToken = default)
    {
        return PostAsync("upload-localizations-from-json", addLocalizationsModels, cancellationToken);
    }
}
