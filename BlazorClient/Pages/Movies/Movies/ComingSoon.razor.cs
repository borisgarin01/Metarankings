using Domain.Movies;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class ComingSoon : CancellableComponentBase
{
    private IEnumerable<Movie> movies = Enumerable.Empty<Movie>();

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
        Movies = await MoviesWebManager.GetAllAsync(DisposalToken) ?? Enumerable.Empty<Movie>();
    }
}
