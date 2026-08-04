using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MoviesListPage : ComponentBase
{
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public IEnumerable<Movie> Movies { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>(@"/api/movies/movies");
    }
}
