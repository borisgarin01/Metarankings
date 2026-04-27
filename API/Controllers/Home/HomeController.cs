using BlazorClient.Components.PagesComponents.Home;
using Data.Repositories.Interfaces.Derived;
using Domain.Common;
using Domain.Games;
using Domain.Movies;
using Domain.Reviews;
using ViewModels;

namespace API.Controllers.Home;

[ApiController]
[Route("api/[controller]")]
public sealed class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    private readonly IGamesPlayersReviewsRepository _gamesPlayersReviewsRepository;
    private readonly IGamesRepository _gamesRepository;

    public HomeController(IGamesPlayersReviewsRepository gamesPlayersReviewsRepository, ILogger<HomeController> logger, IGamesRepository gamesRepository)
    {
        _gamesPlayersReviewsRepository = gamesPlayersReviewsRepository;
        _logger = logger;
        _gamesRepository = gamesRepository;
    }

    [HttpGet("collection-items/{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<CollectionsItemComponent>>> GetCollectionItemsComponents(int pageNumber, int pageSize)
    {
        return Ok(new CollectionsItemComponent[]
        {
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/samye-slozhnye-igry/",
                Title="Самые сложные игры",
                ImageAlt="Самые сложные игры",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/slozhnye-igry-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/best-films-pro-zhenshhin/",
                Title="Лучшие фильмы про женщин",
                ImageAlt="Лучшие фильмы про женщин",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/films-pro-zhenshhin-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/filmy-pro-realnye-sobytiya/",
                Title="Фильмы про реальные события",
                ImageAlt="Фильмы про реальные события",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/pro-realnye-sobytiya-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/luchshie-filmy-pro-monstrov/",
                Title="Лучшие фильмы про монстров",
                ImageAlt="Лучшие фильмы про монстров",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/filmy-pro-monstrov-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/samye-strashnye-igry/",
                Title="Самые страшные игры",
                ImageAlt="Самые страшные игры",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/strashnye-igry-445x250.jpg"
            },
            new CollectionsItemComponent
            {
                Href="https://metarankings.ru/best-games-open-world/",
                Title="Лучшие игры с открытым миром",
                ImageAlt="Лучшие игры с открытым миром",
                ImageSource="https://metarankings.ru/images/uploads/2023/06/best-igry-s-otkrytym-mirom-445x250.jpg"
            }
        });
    }

    [HttpGet("soon-at-cinemas")]
    public async Task<ActionResult<IEnumerable<SoonAtCinemasItemComponent>>> GetSoonAtCinemasItemsComponents()
    {
        return Ok(new SoonAtCinemasItemComponent[]
    {
        new SoonAtCinemasItemComponent
        {
            Href = "https://metarankings.ru/balerina-2025/",
            ImageAlt = "Балерина",
            ImageSource = "https://metarankings.ru/images/uploads/2025/06/balerina-2025-cover-art-50x70.jpg",
            OriginalName = "Ballerina",
            ReleaseDate = new DateTime(2025, 6, 5),
            Title = "Фильм Балерина"
        },
        new SoonAtCinemasItemComponent
        {
            Href = "https://metarankings.ru/kloun-na-kukuruznom-pole-2025/",
            ImageAlt = "Клоун на кукурузном поле",
            ImageSource = "https://metarankings.ru/images/uploads/2025/06/kloun-na-kukuruznom-pole-cover-art-50x70.jpg",
            OriginalName = "Clown in a Cornfield",
            ReleaseDate = new DateTime(2025, 6, 12),
            Title = "Фильм Клоун на кукурузном поле"
        },
        new SoonAtCinemasItemComponent
        {
            Href = "https://metarankings.ru/zombi-kenguru-2025/",
            ImageAlt = "Зомби-кенгуру",
            ImageSource = "https://metarankings.ru/images/uploads/2025/02/zombi-kenguru-cover-art-50x70.jpg",
            OriginalName = "Rippy",
            ReleaseDate = new DateTime(2025, 6, 12),
            Title = "Фильм Зомби-кенгуру"
        },
        new SoonAtCinemasItemComponent
        {
            Href = "https://metarankings.ru/sinister-iz-tmy-2025/",
            ImageAlt = "Синистер. Из тьмы",
            ImageSource = "https://metarankings.ru/images/uploads/2025/06/sinister-iz-tmy-cover-art-50x70.jpg",
            OriginalName = "Ur mörkret",
            ReleaseDate = new DateTime(2025, 6, 12),
            Title = "Фильм Синистер. Из тьмы"
        },
        new SoonAtCinemasItemComponent
        {
            Href = "https://metarankings.ru/igra-v-kalmara-perezagruzka-2025/",
            ImageAlt = "Игра в кальмара: Перезагрузка",
            ImageSource = "https://metarankings.ru/images/uploads/2025/05/igra-v-kalmara-perezagruzka-cover-art-50x70.jpg",
            OriginalName = "Exit",
            ReleaseDate = new DateTime(2025, 6, 19),
            Title = "Фильм Игра в кальмара: Перезагрузка"
        }
    });
    }

    [HttpGet("movies-reviews")]
    public async Task<ActionResult<IEnumerable<MovieReviewListViewModel>>> GetMoviesReviews()
    {
        var moviesReviews = new MovieReview[]
        {
            new MovieReview
            {
                Id=1,
                ApplicationUser=null,
                Date=default,
                Movie=new Movie
                {
                    Name="Фильм 1",
                    Description=string.Empty,
                    ImageSource=string.Empty,
                    OriginalName=string.Empty,
                    Id=1,
                    MovieGenres=new List<MovieGenre>(),
                    MovieReviews=new List<MovieReview>(),
                    MoviesDirectors=new List<MovieDirector>(),
                    MoviesStudios=new List<MovieStudio>(),
                    PremierDate=default,
                    Score=default,
                    ScoresCount=default
                },
                MovieId=1,
                Score=0,
                TextContent="Тест",
                ViewerId=1
            },
            new MovieReview
            {
                Id=2,
                ApplicationUser=null,
                Date=default,
                Movie=new Movie
                {
                    Name="Фильм 2",
                    Description=string.Empty,
                    ImageSource=string.Empty,
                    OriginalName=string.Empty,
                    Id=1,
                    MovieGenres=new List<MovieGenre>(),
                    MovieReviews=new List<MovieReview>(),
                    MoviesDirectors=new List<MovieDirector>(),
                    MoviesStudios=new List<MovieStudio>(),
                    PremierDate=default,
                    Score=default,
                    ScoresCount=default
                },
                MovieId=1,
                Score=0,
                TextContent="Тест",
                ViewerId=1
            },
            new MovieReview
            {
                Id=2,
                ApplicationUser=null,
                Date=default,
                Movie=new Movie
                {
                    Name="Фильм 3",
                    Description=string.Empty,
                    ImageSource=string.Empty,
                    OriginalName=string.Empty,
                    Id=1,
                    MovieGenres=new List<MovieGenre>(),
                    MovieReviews=new List<MovieReview>(),
                    MoviesDirectors=new List<MovieDirector>(),
                    MoviesStudios=new List<MovieStudio>(),
                    PremierDate=default,
                    Score=default,
                    ScoresCount=default
                },
                MovieId=3,
                Score=0,
                TextContent="Тест",
                ViewerId=1
            },
            new MovieReview
            {
                Id=2,
                ApplicationUser=null,
                Date=default,
                Movie=new Movie
                {
                    Name="Фильм 4",
                    Description=string.Empty,
                    ImageSource=string.Empty,
                    OriginalName=string.Empty,
                    Id=1,
                    MovieGenres=new List<MovieGenre>(),
                    MovieReviews=new List<MovieReview>(),
                    MoviesDirectors=new List<MovieDirector>(),
                    MoviesStudios=new List<MovieStudio>(),
                    PremierDate=default,
                    Score=default,
                    ScoresCount=default
                },
                MovieId=4,
                Score=0,
                TextContent="Тест",
                ViewerId=1
            },
            new MovieReview
            {
                Id=2,
                ApplicationUser=null,
                Date=default,
                Movie=new Movie
                {
                    Name="Фильм 5",
                    Description=string.Empty,
                    ImageSource=string.Empty,
                    OriginalName=string.Empty,
                    Id=1,
                    MovieGenres=new List<MovieGenre>(),
                    MovieReviews=new List<MovieReview>(),
                    MoviesDirectors=new List<MovieDirector>(),
                    MoviesStudios=new List<MovieStudio>(),
                    PremierDate=default,
                    Score=default,
                    ScoresCount=default
                },
                MovieId=5,
                Score=0,
                TextContent="Тест",
                ViewerId=1
            }
        };

        return Ok(new MovieReviewListViewModel[]
        {
            new MovieReviewListViewModel(1,"Фильм 1"),
            new MovieReviewListViewModel(2,"Фильм 2"),
            new MovieReviewListViewModel(3,"Фильм 3"),
            new MovieReviewListViewModel(4,"Фильм 4"),
            new MovieReviewListViewModel(5,"Фильм 5")
        });
    }

    [HttpGet("games-reviews/{pageNumber:long}/{pageSize:long}")]
    public async Task<ActionResult<IEnumerable<GameReviewListViewModel>>> GetGamesReviewsAsync(long pageNumber, long pageSize)
    {
        try
        {
            IEnumerable<GameReview> gamesReviews = await _gamesPlayersReviewsRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);

            return Ok(gamesReviews.Select(b => new GameReviewListViewModel(b.GameId, b.Game.Name)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, ex.StackTrace);
            return StatusCode(500, new { ex.Message, ex.StackTrace });
        }
    }

    [HttpGet("nearest/{offset}")]
    public async Task<ActionResult<IEnumerable<GamesReleaseDateItemViewModel>>> GetNearestAsync(short limit)
    {
        IEnumerable<Game> games = await _gamesRepository.GetNearestAsync(limit);

        var gamesReleaseDatetItemViewModels = games.Select(b => new GamesReleaseDateItemViewModel($"/games/Details/{b.Id}", b.Name, b.Image, b.Name, b.Name, b.Platforms.Select(c => new Link(c.Name, $"/platforms/{c.Id}")).ToArray(), b.Genres.Select(c => new Link(c.Name, $"/genres/{c.Id}")).ToArray(), b.ReleaseDate.HasValue ? b.ReleaseDate.Value : DateTime.Today.AddYears(5)));

        return Ok(gamesReleaseDatetItemViewModels);
    }

    [HttpGet("games-release-dates")]
    public async Task<ActionResult<IEnumerable<GamesReleaseDateItemComponent>>> GetGamesReleasesDatesAsync(long pageNumber, long pageSize)
    {
        IEnumerable<GamesReleaseDateItemViewModel> gamesReleaseDateItemsComponents = new GamesReleaseDateItemViewModel[]
        {
            new GamesReleaseDateItemViewModel("https://metarankings.ru/dying-light-the-beast/","Игра Dying Light: The Beast", "https://metarankings.ru/images/uploads/2025/07/dying-light-the-beast-boxart-cover-50x70.jpg", "Dying Light: The Beast", "Dying Light: The Beast", new Link[]{new Link("PC", "https://metarankings.ru/meta/games/pc/"),
                    new Link("PS5", "https://metarankings.ru/meta/games/ps5/"),
                    new Link("Xbox Series X", "https://metarankings.ru/meta/games/xbox-series-x/")}, new Link[]
                {
                    new Link("РПГ", "https://metarankings.ru/genre/rpg/"),
                    new Link("Хоррор", "https://metarankings.ru/genre/xorror/"),
                    new Link("Экшен", "https://metarankings.ru/genre/ekshen/")
                }, new DateTime(2025,6,1)),
            new GamesReleaseDateItemViewModel("https://metarankings.ru/donkey-kong-bananza/","Игра Donkey Kong Bananza", "https://metarankings.ru/images/uploads/2025/07/donkey-kong-bananza-boxart-cover-50x70.jpg", "Donkey Kong Bananza", "Donkey Kong Bananza", new Link[]{new Link("Switch 2", "https://metarankings.ru/meta/games/switch-2/") }, new Link[]
                {
                    new Link("РПГ", "https://metarankings.ru/genre/rpg/"),
                    new Link("Хоррор", "https://metarankings.ru/genre/xorror/"),
                    new Link("Экшен", "https://metarankings.ru/genre/ekshen/")
                },new DateTime(2025,7,17)),
            new GamesReleaseDateItemViewModel("https://metarankings.ru/mindseye/","Игра MindsEye", "https://metarankings.ru/images/uploads/2025/08/mindseye-boxart-cover-50x70.jpg", "MindsEye", "MindsEye",
            new Link[]
            {new Link("PC", "https://metarankings.ru/meta/games/pc/"),
                    new Link("PS5", "https://metarankings.ru/meta/games/ps5/"),
                    new Link("Xbox Series X", "https://metarankings.ru/meta/games/xbox-series-x/")}, new Link[]
                {
                    new Link("Шутер", "https://metarankings.ru/genre/shuter/"),
                    new Link("Приключение", "https://metarankings.ru/genre/priklyuchenie/"),
                    new Link("Экшен", "https://metarankings.ru/genre/ekshen/")
                },new DateTime(2025, 6, 1)),
            new GamesReleaseDateItemViewModel("https://metarankings.ru/shinobi-art-of-vengeance/","Игра SHINOBI: Art of Vengeance","https://metarankings.ru/images/uploads/2025/08/shinobi-art-of-vengeance-boxart-cover-50x70.jpg", "SHINOBI: Art of Vengeance","SHINOBI: Art of Vengeance",new Link[]
                {
                    new Link("Аркада", "https://metarankings.ru/genre/arkada/"),
                    new Link("Платформер", "https://metarankings.ru/genre/platformer/"),
                    new Link("Экшен", "https://metarankings.ru/genre/ekshen/")
                }, new Link[]
                {
                    new Link("PC", "https://metarankings.ru/meta/games/pc/"),
                    new Link("PS5", "https://metarankings.ru/meta/games/ps5/"),
                    new Link("Xbox Series X", "https://metarankings.ru/meta/games/xbox-series-x/")
                }, new DateTime(2025, 8, 26)),
            new GamesReleaseDateItemViewModel("https://metarankings.ru/game-metal-gear-solid-delta-snake-eater/", "Игра Metal Gear Solid Delta: Snake Eater", "https://metarankings.ru/images/uploads/2023/05/metal-gear-solid-delta-snake-eater-boxart-cover-50x70.jpg", "Metal Gear Solid Delta: Snake Eater","Metal Gear Solid Delta: Snake Eater", new Link[]
                {
                    new Link("Экшен", "https://metarankings.ru/genre/ekshen/")
                }, new Link[]
                {
                    new Link("PC", "https://metarankings.ru/meta/games/pc/"),
                    new Link("PS5", "https://metarankings.ru/meta/games/ps5/"),
                    new Link("Xbox Series X", "https://metarankings.ru/meta/games/xbox-series-x/")
                }, new DateTime(2025, 8, 28))
        };

        return Ok(gamesReleaseDateItemsComponents);
    }
}
