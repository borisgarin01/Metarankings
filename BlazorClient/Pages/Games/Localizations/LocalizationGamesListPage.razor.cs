using Domain.Games;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

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
            Localization = await HttpClient.GetFromJsonAsync<Localization>($"/api/Localizations/{LocalizationId}");
        }
        else
        {
            Localization = await HttpClient.GetFromJsonAsync<Localization>($"/api/Localizations/{LocalizationId}/{PlatformId}");
        }
    }
}
