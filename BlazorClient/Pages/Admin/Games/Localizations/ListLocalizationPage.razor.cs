using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Localizations;

public partial class ListLocalizationPage : ComponentBase
{
    public IEnumerable<Localization> Localizations { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Localizations = await HttpClient.GetFromJsonAsync<IEnumerable<Localization>>(@"/api/Games/Localizations");
    }
}
