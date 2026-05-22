using Domain.Movies;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public sealed partial class NewMovies : ComponentBase
{
    private IEnumerable<Movie> movies;

    public IEnumerable<Movie> Movies
    {
        get => movies;
        private set
        {
            movies = value;
            StateHasChanged();
        }
    }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Movies = await MoviesWebManager.GetAllAsync();
    }
}
