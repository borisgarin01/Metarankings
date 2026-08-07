using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.RequestsModels.Games.Platforms;
using WebManagers;

namespace BlazorClient.Pages.Games.Localizations;

public partial class LocalizationComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public Localization Localization { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    public IEnumerable<Platform> Platforms { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Platforms = await PlatformsWebManager.GetAllAsync();
    }
}
