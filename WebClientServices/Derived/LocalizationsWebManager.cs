using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class LocalizationsWebManager : WebManager, IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel>
{
    public LocalizationsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddLocalizationModel addLocalizationModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Localizations", addLocalizationModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Localizations/localizations-excel-upload", formFile);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddLocalizationModel> addLocalizationsModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Localizations/upload-localizations-from-json", addLocalizationsModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Localizations/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Localization>> GetAllAsync()
    {
        var localizations = await HttpClient.GetFromJsonAsync<IEnumerable<Localization>>("/api/Localizations");
        return localizations;
    }

    public async Task<IEnumerable<Localization>> GetAllAsync(long offset, long limit)
    {
        var localizations = await HttpClient.GetFromJsonAsync<IEnumerable<Localization>>($"/api/Localizations/{offset}/{limit}");
        return localizations;
    }

    public async Task<Localization> GetAsync(long id)
    {
        var localization = await HttpClient.GetFromJsonAsync<Localization>($"/api/Localizations/{id}");
        return localization;
    }

    public async Task<Localization> UpdateAsync(long id, UpdateLocalizationModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateLocalizationModel>($"/api/Localizations/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<Localization>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }
}
