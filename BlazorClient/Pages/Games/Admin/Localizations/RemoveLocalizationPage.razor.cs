using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using WebManagers;
using WebManagers.Derived;

namespace BlazorClient.Pages.Games.Admin.Localizations;

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
            NavigationManager.NavigateTo("/admin/localizations/list-localizations");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
