using Domain.Games;
using Domain.Movies;
using Domain.Reviews;

namespace BlazorClient.Pages;

public partial class Home : ComponentBase
{
    private IEnumerable<Game> games;
    private IEnumerable<GameReview> gamesReviews;

    private IEnumerable<Movie> movies;
    private IEnumerable<MovieReview> moviesReviews;

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

    public IEnumerable<GameReview> GamesReviews
    {
        get => gamesReviews;
        private set
        {
            gamesReviews = value;
            StateHasChanged();
        }
    }

    public IEnumerable<MovieReview> MoviesReviews
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

    protected override async Task OnInitializedAsync()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        if (PageSize < 1)
            PageSize = 5;
        // Fetch data based on the current PageSize and PageNumber

        var gamesGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/{PageNumber}/{PageSize}");
        var gamesGamersReviewsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<GameReview>>($"/api/GamesGamersReviews/{GamesGamersReviewsOffset}/{GamesGamersReviewsLimit}");
        var moviesGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Movie>>($"/api/Movies/{PageNumber}/{PageSize}");
        var moviesViewersReviewsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<MovieReview>>($"/api/MoviesViewersReviews/{MoviesViewersReviewsOffset}/{MoviesViewersReviewsLimit}");

        await Task.WhenAll(gamesGettingTask, gamesGamersReviewsGettingTask, moviesGettingTask, moviesViewersReviewsGettingTask)
            .ContinueWith(b =>
        {
            Games = gamesGettingTask.Result;
            GamesReviews = gamesGamersReviewsGettingTask.Result;
            Movies = moviesGettingTask.Result;
            MoviesReviews = moviesViewersReviewsGettingTask.Result;
        });
    }
}
