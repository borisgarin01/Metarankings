using Domain.Games;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres;

public partial class ListMoviesGenresPage : ComponentBase
{
    public IEnumerable<Genre> Genres { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genres = await HttpClient.GetFromJsonAsync<IEnumerable<Genre>>("/api/movies/MoviesGenres");
    }
}
