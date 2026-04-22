using BlazorClient.Components.PagesComponents.Home;
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

    public IEnumerable<GamesReleaseDateItemComponent> GamesReleaseDateItemComponents { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        if (PageSize < 1)
            PageSize = 5;
        // Fetch data based on the current PageSize and PageNumber

        var gamesGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/First/{PageNumber}/{PageSize}");
        var gamesGamersReviewsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<GameReview>>($"/api/Games/GamesGamersReviews/{GamesGamersReviewsOffset}/{GamesGamersReviewsLimit}");
        var moviesGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<Movie>>($"/api/Movies/Movies/{PageNumber}/{PageSize}");
        var moviesViewersReviewsGettingTask = HttpClient.GetFromJsonAsync<IEnumerable<MovieReview>>($"/api/Movies/MoviesViewersReviews/{MoviesViewersReviewsOffset}/{MoviesViewersReviewsLimit}");

        await Task.WhenAll(gamesGettingTask, gamesGamersReviewsGettingTask, moviesGettingTask, moviesViewersReviewsGettingTask)
            .ContinueWith(b =>
        {
            Games = gamesGettingTask.Result;
            GamesReviews = gamesGamersReviewsGettingTask.Result;
            Movies = moviesGettingTask.Result;
            MoviesReviews = moviesViewersReviewsGettingTask.Result;
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
                    Genres=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/genre/rpg/", "РПГ" },
                        {"https://metarankings.ru/genre/xorror/", "Хоррор" },
                        {"https://metarankings.ru/genre/ekshen/", "Экшен" }
                    },
                    Platforms=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/meta/games/pc/", "PC"},
                        {"https://metarankings.ru/meta/games/ps5/", "PS5" },
                        {"https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X" }
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
                    Genres=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/genre/arkada/", "Аркада" },
                        {"https://metarankings.ru/genre/priklyuchenie/", "Приключение" }
                    },
                    Platforms=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/meta/games/switch-2/", "Switch 2"}
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
                    Genres=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/genre/rpg/", "РПГ" },
                        {"https://metarankings.ru/genre/shuter/", "Шутер" },
                        {"https://metarankings.ru/genre/ekshen/", "Экшен" }
                    },
                    Platforms=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/meta/games/pc/", "PC"},
                        {"https://metarankings.ru/meta/games/ps5/", "PS5" },
                        {"https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X" }
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
                    Genres=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/genre/arkada/", "Аркада" },
                        {"https://metarankings.ru/genre/platformer/", "Платформер" },
                        {"https://metarankings.ru/genre/ekshen/", "Экшен" }
                    },
                    Platforms=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/meta/games/pc/", "PC"},
                        {"https://metarankings.ru/meta/games/ps5/", "PS5" },
                        {"https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X" },
                        {"https://metarankings.ru/meta/games/switch/", "Switch"}
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
                    Genres=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/genre/ekshen/", "Экшен" }
                    },
                    Platforms=new Dictionary<string, string>
                    {
                        {"https://metarankings.ru/meta/games/pc/", "PC"},
                        {"https://metarankings.ru/meta/games/ps5/", "PS5" },
                        {"https://metarankings.ru/meta/games/xbox-series-x/", "Xbox Series X" }
                    }
                }
            };
        });
    }
}
