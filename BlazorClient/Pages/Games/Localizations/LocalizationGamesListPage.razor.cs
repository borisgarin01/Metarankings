using Domain.Games;

namespace BlazorClient.Pages.Games.Localizations;

public partial class LocalizationGamesListPage : ComponentBase
{
    [Parameter]
    public long LocalizationId { get; set; }
    public Localization Localization { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [SupplyParameterFromQuery]
    [Parameter]
    public int? PlatformId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (PlatformId is null)
        {
            Localization = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Localization>($"/api/Games/Localizations/{LocalizationId}");
        }
        else
        {
            Localization = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Localization>($"/api/Games/Localizations/{LocalizationId}/{PlatformId}");
        }
    }
}
