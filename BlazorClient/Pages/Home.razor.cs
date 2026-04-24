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

    public IEnumerable<GamesReleaseDateItemComponent> GamesReleaseDateItemComponents { get; private set; }
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
        var collectionsItemsComponentGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<CollectionsItemComponent>>("api/home/collection-items");
        var soonAtCinemasItemsComponents = HttpClient.GetFromJsonAsync<IEnumerable<SoonAtCinemasItemComponent>>("api/home/soon-at-cinemas");

        await Task.WhenAll(gamesGettingTask, gamesGamersReviewsGettingTask, moviesGettingTask, moviesViewersReviewsGettingTask, collectionsItemsComponentGettingTask)
            .ContinueWith(b =>
        {
            Games = gamesGettingTask.Result;
            GamesReviews = gamesGamersReviewsGettingTask.Result;
            Movies = moviesGettingTask.Result;
            MoviesReviews = moviesViewersReviewsGettingTask.Result;
            CollectionsItemComponents = collectionsItemsComponentGettingTask.Result;
            SoonAtCinemasItemComponents = soonAtCinemasItemsComponents.Result;
            GamesReleaseDateItemComponents = new GamesReleaseDateItemComponent[]
            {
                new GamesReleaseDateItemComponent
                {
                    Title="Игра Dying Light: The Beast",
                    Href="https://metarankings.ru/dying-light-the-beast/",
                    ImageAlt="Dying Light: The Beast",
                    ImageSource="https://metarankings.ru/images/uploads/2025/07/dying-light-the-beast-boxart-cover-50x70.jpg",
                    ItemName="Dying Light: The Beast",
                    ReleaseDate=new DateTime(2025,6,1),
                    Genres=new Link[]
                    {
                        new Link("https://metarankings.ru/genre/rpg/", "РПГ" ),
                        new Link("https://metarankings.ru/genre/xorror/", "Хоррор"),
                        new Link("https://metarankings.ru/genre/ekshen/", "Экшен")
                    },
                    Platforms=new Link[]
                    {
                        new Link("https://metarankings.ru/meta/games/pc/", "PC"),
                        new Link("https://metarankings.ru/meta/games/ps5/", "PS5"),
                        new Link("https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X")
                    }
                },
                new GamesReleaseDateItemComponent
                {
                    Title="Игра Donkey Kong Bananza",
                    Href="https://metarankings.ru/donkey-kong-bananza/",
                    ImageAlt="Donkey Kong Bananza",
                    ImageSource="https://metarankings.ru/images/uploads/2025/07/donkey-kong-bananza-boxart-cover-50x70.jpg",
                    ItemName="Donkey Kong Bananza",
                    ReleaseDate=new DateTime(2025,7,17),
                    Genres=new Link[]
                    {
                        new Link("https://metarankings.ru/genre/arkada/", "Аркада"),
                        new Link("https://metarankings.ru/genre/priklyuchenie/", "Приключение")
                    },
                    Platforms=new Link[]
                    {
                        new Link("https://metarankings.ru/meta/games/switch-2/", "Switch 2")
                    }
                },
                new GamesReleaseDateItemComponent
                {
                    Title="Игра MindsEye",
                    Href="https://metarankings.ru/mindseye/",
                    ImageAlt="MindsEye",
                    ImageSource="https://metarankings.ru/images/uploads/2025/08/mindseye-boxart-cover-50x70.jpg",
                    ItemName="MindsEye",
                    ReleaseDate=new DateTime(2025,6,1),
                    Genres=new Link[]
                    {
                        new Link("https://metarankings.ru/genre/rpg/", "РПГ"),
                        new Link("https://metarankings.ru/genre/shuter/", "Шутер"),
                        new Link("https://metarankings.ru/genre/ekshen/", "Экшен")
                    },
                    Platforms = new Link[]
                    {
                        new Link("https://metarankings.ru/meta/games/pc/", "PC"),
                        new Link("https://metarankings.ru/meta/games/ps5/", "PS5"),
                        new Link("https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X")
                    }
                },
                new GamesReleaseDateItemComponent
                {
                    Title="Игра SHINOBI: Art of Vengeance",
                    Href="https://metarankings.ru/shinobi-art-of-vengeance/",
                    ImageAlt="SHINOBI: Art of Vengeance",
                    ImageSource="https://metarankings.ru/images/uploads/2025/08/shinobi-art-of-vengeance-boxart-cover-50x70.jpg",
                    ItemName="SHINOBI: Art of Vengeance",
                    ReleaseDate=new DateTime(2025,8,25),
                    Genres = new Link[]
                    {
                        new Link("https://metarankings.ru/genre/arkada/", "Аркада"),
                        new Link("https://metarankings.ru/genre/platformer/", "Платформер"),
                        new Link("https://metarankings.ru/genre/ekshen/", "Экшен")
                    },
                    Platforms = new Link[]
                    {
                        new Link("https://metarankings.ru/meta/games/pc/", "PC"),
                        new Link("https://metarankings.ru/meta/games/ps5/", "PS5"),
                        new Link("https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X"),
                        new Link("https://metarankings.ru/meta/games/switch/", "Switch")
                    }
                },
                new GamesReleaseDateItemComponent
                {
                    Title="Игра Metal Gear Solid Delta: Snake Eater",
                    Href="https://metarankings.ru/game-metal-gear-solid-delta-snake-eater/",
                    ImageAlt="Metal Gear Solid Delta: Snake Eater",
                    ImageSource="https://metarankings.ru/images/uploads/2023/05/metal-gear-solid-delta-snake-eater-boxart-cover-50x70.jpg",
                    ItemName="Metal Gear Solid Delta: Snake Eater",
                    ReleaseDate=new DateTime(2025,8,28),
                    Genres = new Link[]
                    {
                        new Link("https://metarankings.ru/genre/ekshen/", "Экшен")
                    },
                    Platforms = new Link[]
                    {
                        new Link("https://metarankings.ru/meta/games/pc/", "PC"),
                        new Link("https://metarankings.ru/meta/games/ps5/", "PS5" ),
                        new Link("https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X")
                    }
                }
            };
        });
    }
}
