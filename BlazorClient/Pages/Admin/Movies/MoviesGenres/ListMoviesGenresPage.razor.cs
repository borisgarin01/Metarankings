using Domain.Games;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres;

public partial class ListMoviesGenresPage : ComponentBase
{
    public IEnumerable<Genre> Genres { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genres = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Genre>>("/api/movies/MoviesGenres");
    }
}
