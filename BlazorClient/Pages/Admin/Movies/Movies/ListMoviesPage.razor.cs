using Domain.Movies;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Admin.Movies.Movies;

public partial class ListMoviesPage : ComponentBase
{
    public IEnumerable<Movie> Movies { get; private set; }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Movies = await MoviesWebManager.GetAllAsync();
    }
}
