using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

public partial class ListGamesPage : ComponentBase
{
    public IEnumerable<Game> Games { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Games = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Game>>(@"/api/Games/Games");
    }
}
