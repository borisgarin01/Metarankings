using Domain.Games;

namespace BlazorClient.Pages.Games.Localizations;

public partial class LocalizationGamesListPage : ComponentBase
{
    [Parameter]
    public long LocalizationId { get; set; }
    public Localization Localization { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    [SupplyParameterFromQuery]
    [Parameter]
    public int? PlatformId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (PlatformId is null)
        {
            Localization = await HttpClient.GetFromJsonAsync<Localization>($"/api/Games/Localizations/{LocalizationId}");
        }
        else
        {
            Localization = await HttpClient.GetFromJsonAsync<Localization>($"/api/Games/Localizations/{LocalizationId}/{PlatformId}");
        }
    }
}
