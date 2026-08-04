using Domain.Games;

namespace BlazorClient.Pages.Games.Genres;

public partial class GenreGamesListPage : ComponentBase
{
    [Parameter]
    public long GenreId { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public Genre Genre { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Genre = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Genre>($"/api/Games/Genres/{GenreId}");
    }
}
