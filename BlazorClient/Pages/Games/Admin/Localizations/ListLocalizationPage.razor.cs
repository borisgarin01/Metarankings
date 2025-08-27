using Domain.Games;

namespace BlazorClient.Pages.Games.Admin.Localizations;

public partial class ListLocalizationPage : ComponentBase
{
    public IEnumerable<Localization> Localizations { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Localizations = await HttpClient.GetFromJsonAsync<IEnumerable<Localization>>(@"/api/Localizations");
    }
}
