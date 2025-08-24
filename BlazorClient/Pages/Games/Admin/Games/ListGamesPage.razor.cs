using Domain.Games;

namespace BlazorClient.Pages.Games.Admin.Games;

public partial class ListGamesPage : ComponentBase
{
    public IEnumerable<Game> Games { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Games = await HttpClient.GetFromJsonAsync<IEnumerable<Game>>(@"/api/Games");
    }
}
