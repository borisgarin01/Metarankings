using Domain.RequestsModels.Games.Localizations;
using WebManagers;
using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Localizations;

public partial class ListLocalizationPage : ComponentBase
{
    public IEnumerable<Localization> Localizations { get; private set; }

    [Inject]
    public IWebManager<Localization, AddLocalizationModel, UpdateLocalizationModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Localizations = await WebManager.GetAllAsync();
    }
}
