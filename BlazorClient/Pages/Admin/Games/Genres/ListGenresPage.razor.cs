using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Genres;

public partial class ListGenresPage : ComponentBase
{
    public IEnumerable<Genre> Genres { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genres = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Genre>>(@"/api/Games/Genres");
    }
}
