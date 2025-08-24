using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using Microsoft.AspNetCore.Authorization;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Localizations;

[Authorize(Policy = "Admin")]
public partial class AddLocalizationPage : ComponentBase
{
    [Inject]
    public IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel> LocalizationsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddLocalizationModel AddLocalizationModel { get; } = new AddLocalizationModel();

    public async Task AddLocalizationAsync()
    {
        HttpResponseMessage httpResponseMessage = await LocalizationsWebManager.AddAsync(AddLocalizationModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/localizations/list-localizations");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
