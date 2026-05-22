using Domain.Movies;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class ComingSoon : ComponentBase
{
    private IEnumerable<Movie> movies;

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    public IEnumerable<Movie> Movies
    {
        get => movies;
        private set
        {
            movies = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Movies = await MoviesWebManager.GetAllAsync();
    }
}
