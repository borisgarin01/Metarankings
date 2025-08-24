using Domain.Games;
using Domain.RequestsModels.Games.Localizations;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Localizations;

public partial class RemoveLocalizationPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Localization Localization { get; private set; }

    [Inject]
    public IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel> LocalizationsWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Localization = await LocalizationsWebManager.GetAsync(Id);
    }
}
