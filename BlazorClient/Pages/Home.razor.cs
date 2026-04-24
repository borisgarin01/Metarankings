using BlazorClient.Components.PagesComponents.Home;
using Domain.Common;
using Domain.Games;
using Domain.Movies;
using Domain.Reviews;
using ViewModels;

namespace BlazorClient.Pages;

public partial class Home : ComponentBase
{
    private IEnumerable<Game> games;
    private IEnumerable<GameReviewListViewModel> gamesReviews;

    private IEnumerable<Movie> movies;
    private IEnumerable<MovieReviewListViewModel> moviesReviews;

    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<Game> Games
    {
        get => games;
        private set
        {
            games = value;
            StateHasChanged();
        }
    }

    public IEnumerable<Movie> Movies
    {
        get => movies;
        private set
        {
            movies = value;
            StateHasChanged();
        }
    }

    public IEnumerable<GameReviewListViewModel> GamesReviews
    {
        get => gamesReviews;
        private set
        {
            gamesReviews = value;
            StateHasChanged();
        }
    }

    public IEnumerable<MovieReviewListViewModel> MoviesReviews
    {
        get => moviesReviews;
        private set
        {
            moviesReviews = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public int PageSize { get; set; } = 5; // Default value

    [Parameter]
    public int PageNumber { get; set; } = 1; // Default value

    public int GamesGamersReviewsOffset { get; } = 0;
    public int GamesGamersReviewsLimit { get; } = 5;

    public int MoviesViewersReviewsOffset { get; } = 0;
    public int MoviesViewersReviewsLimit { get; } = 5;

    public IEnumerable<GamesReleaseDateItemViewModel> GamesReleaseDateItemComponents { get; private set; }
    public IEnumerable<CollectionsItemComponent> CollectionsItemComponents { get; private set; }
    public IEnumerable<SoonAtCinemasItemComponent> SoonAtCinemasItemComponents { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        if (PageSize < 1)
            PageSize = 5;
        // Fetch data based on the current PageSize and PageNumber

        var gamesGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/First/{PageNumber}/{PageSize}");
        var gamesGamersReviewsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<GameReviewListViewModel>>($"/api/Home/games-reviews/{PageNumber}/{PageSize}");
        var moviesGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Movie>>($"/api/Movies/Movies/{PageNumber}/{PageSize}");
        var moviesViewersReviewsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<MovieReviewListViewModel>>($"/api/Home/movies-reviews");
        var collectionsItemsComponentGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<CollectionsItemComponent>>("/api/home/collection-items");
        var soonAtCinemasItemsComponents = HttpClient.GetFromJsonAsync<IEnumerable<SoonAtCinemasItemComponent>>("/api/home/soon-at-cinemas");
        var nearestGames = HttpClient.GetFromJsonAsync<IEnumerable<GamesReleaseDateItemViewModel>>($"/api/home/nearest/{PageSize}");

        await Task.WhenAll(gamesGettingTask, gamesGamersReviewsGettingTask, moviesGettingTask, moviesViewersReviewsGettingTask, collectionsItemsComponentGettingTask)
            .ContinueWith(b =>
        {
            Games = gamesGettingTask.Result;
            GamesReviews = gamesGamersReviewsGettingTask.Result;
            Movies = moviesGettingTask.Result;
            MoviesReviews = moviesViewersReviewsGettingTask.Result;
            CollectionsItemComponents = collectionsItemsComponentGettingTask.Result;
            SoonAtCinemasItemComponents = soonAtCinemasItemsComponents.Result;
            GamesReleaseDateItemComponents = nearestGames.Result;
        });
    }
}
