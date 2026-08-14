using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MoviesListPage : ComponentBase
{
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public IEnumerable<Movie> Movies { get; private set; }

    [SupplyParameterFromQuery]
    public long? GenreId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (GenreId is null)
            Movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>("/api/movies/movies");
        else
            Movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>($"/api/movies/movies?genreId={GenreId}");
    }
}
