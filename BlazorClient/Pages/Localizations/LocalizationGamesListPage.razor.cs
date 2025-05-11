using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Localizations;

public partial class LocalizationGamesListPage : ComponentBase
{
    [Parameter]
    public long LocalizationId { get; set; }
    public Localization Localization { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Localization = await HttpClient.GetFromJsonAsync<Localization>($"/api/Localizations/{LocalizationId}");
    }
}
