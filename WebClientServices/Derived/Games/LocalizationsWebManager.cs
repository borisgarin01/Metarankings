using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Games;

public sealed class LocalizationsWebManager : CrudWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    public LocalizationsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Localizations")
    {
    }

    public override Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        return PostAsync("localizations-excel-upload", formFile);
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddLocalizationModel> addLocalizationsModels)
    {
        return PostAsync("upload-localizations-from-json", addLocalizationsModels);
    }
}
