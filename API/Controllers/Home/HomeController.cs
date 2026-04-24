using BlazorClient.Components.PagesComponents.Home;
using Domain.Games;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.Reviews;
using ViewModels;

namespace API.Controllers.Home;

[ApiController]
[Route("api/[controller]")]
public sealed class HomeController : ControllerBase
{
    [HttpGet("collection-items")]
    public async Task<ActionResult<IEnumerable<CollectionsItemComponent>>> GetCollectionItemsComponents()
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

    [HttpGet("games-reviews")]
    public async Task<ActionResult<IEnumerable<GameReviewListViewModel>>> GetGamesReviews()
    {
        var gamesReviews = new GameReview[]
        {
            new GameReview
            {
                Id=1,
                GameId=1,
                Game=new Game
                {
                    Name="The Last of Us",
                    Description="Тест",
                    Developers=new List<Developer>(),
                    GamesPlayersReviews=new List<GameReview>(),
                    Genres=new List<Genre>(),
                    Id=1,
                    Image="Image",
                    Localization=new Localization(),
                    LocalizationId=1,
                    Platforms=new List<Platform>(),
                    Publishers=new List<Publisher>(),
                    ReleaseDate=default,
                    Screenshots=new List<GameScreenshot>(),
                    GameCollections=new List<GameCollection>(),
                    Trailer=""
                }
            },
            new GameReview
            {
                Id=1,
                GameId=1,
                Game=new Game
                {
                    Name="Uncharted 4: A Thief's End",
                    Description="Тест",
                    Developers=new List<Developer>(),
                    GamesPlayersReviews=new List<GameReview>(),
                    Genres=new List<Genre>(),
                    Id=1,
                    Image="Image",
                    Localization=new Localization(),
                    LocalizationId=1,
                    Platforms=new List<Platform>(),
                    Publishers=new List<Publisher>(),
                    ReleaseDate=default,
                    Screenshots=new List<GameScreenshot>(),
                    GameCollections=new List<GameCollection>(),
                    Trailer=""
                }
            },
            new GameReview
            {
                Id=1,
                GameId=1,
                Game=new Game
                {
                    Name="God of War",
                    Description="Тест",
                    Developers=new List<Developer>(),
                    GamesPlayersReviews=new List<GameReview>(),
                    Genres=new List<Genre>(),
                    Id=1,
                    Image="Image",
                    Localization=new Localization(),
                    LocalizationId=1,
                    Platforms=new List<Platform>(),
                    Publishers=new List<Publisher>(),
                    ReleaseDate=default,
                    Screenshots=new List<GameScreenshot>(),
                    GameCollections=new List<GameCollection>(),
                    Trailer=""
                }
            },
            new GameReview
            {
                Id=1,
                GameId=1,
                Game=new Game
                {
                    Name="Detroit: Become Human",
                    Description="Тест",
                    Developers=new List<Developer>(),
                    GamesPlayersReviews=new List<GameReview>(),
                    Genres=new List<Genre>(),
                    Id=1,
                    Image="Image",
                    Localization=new Localization(),
                    LocalizationId=1,
                    Platforms=new List<Platform>(),
                    Publishers=new List<Publisher>(),
                    ReleaseDate=default,
                    Screenshots=new List<GameScreenshot>(),
                    GameCollections=new List<GameCollection>(),
                    Trailer=""
                }
            },
            new GameReview
            {
                Id=1,
                GameId=1,
                Game=new Game
                {
                    Name="Valiant Hearts",
                    Description="Тест",
                    Developers=new List<Developer>(),
                    GamesPlayersReviews=new List<GameReview>(),
                    Genres=new List<Genre>(),
                    Id=1,
                    Image="Image",
                    Localization=new Localization(),
                    LocalizationId=1,
                    Platforms=new List<Platform>(),
                    Publishers=new List<Publisher>(),
                    ReleaseDate=default,
                    Screenshots=new List<GameScreenshot>(),
                    GameCollections=new List<GameCollection>(),
                    Trailer=""
                }
            }
        };

        return Ok(gamesReviews.Select(b => new GameReviewListViewModel(b.Id, b.Game.Name)));
    }
}
