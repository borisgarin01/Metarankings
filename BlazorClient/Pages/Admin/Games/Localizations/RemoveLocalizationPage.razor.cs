using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Localizations;

public partial class RemoveLocalizationPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Localization Localization { get; private set; }

    [Inject]
    public IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel> LocalizationsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Localization = await LocalizationsWebManager.GetAsync(Id);
    }

    public async Task RemoveLocalizationAsync()
    {
        HttpResponseMessage httpResponseMessage = await LocalizationsWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/Games/localizations/list-localizations");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
