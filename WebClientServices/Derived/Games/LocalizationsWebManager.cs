using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class LocalizationsWebManager : WebManager, IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    public LocalizationsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddLocalizationModel addLocalizationModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Localizations", addLocalizationModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Localizations/localizations-excel-upload", formFile);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddLocalizationModel> addLocalizationsModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Localizations/upload-localizations-from-json", addLocalizationsModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Games/Localizations/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Localization>> GetAllAsync()
    {
        IEnumerable<Localization>? localizations = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Localization>>("/api/Games/Localizations");
        return localizations;
    }

    public async Task<IEnumerable<Localization>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Localization>? localizations = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Localization>>($"/api/Games/Localizations/{offset}/{limit}");
        return localizations;
    }

    public async Task<Localization> GetAsync(long id)
    {
        Localization? localization = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Localization>($"/api/Games/Localizations/{id}");
        return localization;
    }

    public async Task<Localization> UpdateAsync(long id, UpdateLocalizationModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Games/Localizations/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Localization>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Localization>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
