using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Localizations;

public partial class ListLocalizationPage : ComponentBase
{
    public IEnumerable<Localization> Localizations { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Localizations = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Localization>>(@"/api/Games/Localizations");
    }
}
