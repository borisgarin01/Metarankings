using BlazorClient.Components.PagesComponents.Home;
using Domain.Games;
using Domain.Movies;
using Domain.Reviews;
using ViewModels;
using WMBlazorSlickCarousel.WMBSC;

namespace BlazorClient.Pages;

public partial class Home : ComponentBase
{
    private IEnumerable<Game> games;
    private IEnumerable<GameReview> gamesReviews;
    private IEnumerable<Movie> movies;
    private IEnumerable<MovieViewerReview> moviesReviews;
    private IEnumerable<CollectionsItemComponent> collectionsItemsComponents;
    private IEnumerable<SoonAtCinemasItemComponent> soonAtCinemasItemComponents;
    private IEnumerable<GamesReleaseDateItemViewModel> gamesReleaseDateItemComponents;
    private IEnumerable<Domain.Movies.Genre> moviesGenres;
    private WMBSCInitialSettings configurations;
    private BlazorSlickCarousel theCarousel;

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

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

    public IEnumerable<Domain.Movies.Genre> MoviesGenres
    {
        get => moviesGenres;
        set
        {
            moviesGenres = value;
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

    public IEnumerable<MovieViewerReview> MoviesReviews
    {
        get => moviesReviews;
        private set
        {
            moviesReviews = value;
            StateHasChanged();
        }
    }

    public IEnumerable<CollectionsItemComponent> CollectionsItemComponents
    {
        get => collectionsItemsComponents;
        set
        {
            collectionsItemsComponents = value;
            StateHasChanged();
        }
    }

    public IEnumerable<SoonAtCinemasItemComponent> SoonAtCinemasItemComponents
    {
        get => soonAtCinemasItemComponents;
        set
        {
            soonAtCinemasItemComponents = value;
            StateHasChanged();
        }
    }

    public IEnumerable<GamesReleaseDateItemViewModel> GamesReleaseDateItemComponents
    {
        get => gamesReleaseDateItemComponents;
        set
        {
            gamesReleaseDateItemComponents = value;
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
        configurations = new WMBSCInitialSettings
        {
            arrows = true,          // Показывать стрелки
            dots = true,            // Показывать точки навигации
            slidesToShow = 5,       // <- ОТОБРАЖАТЬ 5 ИЗОБРАЖЕНИЯ В РАЗВОРОТЕ
            slidesToScroll = 5,     // Пролистывать по 5 изображения за раз
            infinite = false
        };

        if (PageNumber < 1)
            PageNumber = 1;
        if (PageSize < 1)
            PageSize = 5;

        var httpClient = HttpClientFactory.CreateClient("AuthorizedClient");

        // Fetch data based on the current PageSize and PageNumber
        Task<IEnumerable<Game>?> gamesGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/First/{PageNumber}/{PageSize}");
        Task<IEnumerable<GameReview>?> gamesGamersReviewsGettingTask = httpClient.GetFromJsonAsync<IEnumerable<GameReview>>($"/api/Games/GamesGamersReviews/{GamesGamersReviewsOffset}/{GamesGamersReviewsLimit}");
        Task<IEnumerable<Movie>?> moviesGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Movie>>($"/api/movies/{PageNumber}/{PageSize}");
        Task<IEnumerable<MovieViewerReview>?> moviesViewersReviewsGettingTask = httpClient.GetFromJsonAsync<IEnumerable<MovieViewerReview>>($"/api/Movies/MoviesViewersReviews/{MoviesViewersReviewsOffset}/{MoviesViewersReviewsLimit}");
        Task<IEnumerable<CollectionsItemComponent>> collectionsItemsComponents = httpClient.GetFromJsonAsync<IEnumerable<CollectionsItemComponent>>($"/api/home/collections/{PageNumber}/{PageSize}");
        Task<IEnumerable<SoonAtCinemasItemComponent>> soonAtCinemasItemComponents = httpClient.GetFromJsonAsync<IEnumerable<SoonAtCinemasItemComponent>>($"/api/home/soon-at-cinemas");
        Task<IEnumerable<GamesReleaseDateItemViewModel>> gamesReleaseDateItemComponents = httpClient.GetFromJsonAsync<IEnumerable<GamesReleaseDateItemViewModel>>($"/api/home/games-release-dates/{PageNumber}/{PageSize}");
        Task<IEnumerable<Domain.Movies.Genre>> moviesGenresGettingTask = httpClient.GetFromJsonAsync<IEnumerable<Domain.Movies.Genre>>($"/api/home/games-release-dates/{PageNumber}/{PageSize}");

        // Wait for ALL tasks to complete
        await Task.WhenAll(
            gamesGettingTask,
            gamesGamersReviewsGettingTask,
            moviesGettingTask,
            moviesViewersReviewsGettingTask,
            collectionsItemsComponents,
            soonAtCinemasItemComponents,  // Added
            gamesReleaseDateItemComponents,
            moviesGenresGettingTask// Added
        );

        // Then assign all results
        Games = gamesGettingTask.Result;
        GamesReviews = gamesGamersReviewsGettingTask.Result;
        Movies = moviesGettingTask.Result;
        MoviesReviews = moviesViewersReviewsGettingTask.Result;
        CollectionsItemComponents = collectionsItemsComponents.Result;
        SoonAtCinemasItemComponents = soonAtCinemasItemComponents.Result;
        GamesReleaseDateItemComponents = gamesReleaseDateItemComponents.Result;
        MoviesGenres = moviesGenresGettingTask.Result;
    }
}
