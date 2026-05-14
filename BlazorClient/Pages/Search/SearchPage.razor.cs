
using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies.Movies;
using WebManagers;
using WebManagers.Derived.Games;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Search;

public partial class SearchPage : ComponentBase
{
    private IEnumerable<Movie> movies;
    private IEnumerable<Game> games;

    [SupplyParameterFromQuery]
    public string Text { get; set; }

    public IEnumerable<Movie> Movies
    {
        get => movies;
        private set
        {
            movies = value;
            StateHasChanged();
        }
    }

    public IEnumerable<Game> Games
    {
        get => games;
        private set
        {
            games = value;
            StateHasChanged();
        }
    }

    [Inject]
    public GamesWebManager GamesWebManager { get; set; }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<Movie>> moviesSearchingByNameTask = MoviesWebManager.SearchByName(Text);
        Task<IEnumerable<Game>> gamesSearchingByNameTask = GamesWebManager.SearchByName(Text);

        await Task.WhenAll(moviesSearchingByNameTask, gamesSearchingByNameTask).ContinueWith(b =>
        {
            Movies = moviesSearchingByNameTask.Result;
            Games = gamesSearchingByNameTask.Result;
        });
    }
}
