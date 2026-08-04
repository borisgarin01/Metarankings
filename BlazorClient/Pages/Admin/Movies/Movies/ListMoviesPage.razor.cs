using Domain.Movies;

namespace BlazorClient.Pages.Admin.Movies.Movies;

public partial class ListMoviesPage : ComponentBase
{
    public IEnumerable<Movie> Movies { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Movies = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Movie>>(@"/api/Movies/Movies");
    }
}
