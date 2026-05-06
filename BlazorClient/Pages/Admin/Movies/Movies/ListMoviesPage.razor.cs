using Domain.Movies;

namespace BlazorClient.Pages.Admin.Movies.Movies;

public partial class ListMoviesPage : ComponentBase
{
    public IEnumerable<Movie> Movies { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Movies = await HttpClient.GetFromJsonAsync<IEnumerable<Movie>>(@"/api/Movies/Movies");
    }
}
