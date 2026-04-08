using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Genres;

public partial class ListGenresPage : ComponentBase
{
    public IEnumerable<Genre> Genres { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genres = await HttpClient.GetFromJsonAsync<IEnumerable<Genre>>(@"/api/Games/Genres");
    }
}
