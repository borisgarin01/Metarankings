using BlazorClient.Components.PagesComponents.Home;
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
        return Ok(new GameReviewListViewModel[]
        {
            new GameReviewListViewModel(1,"Игра 1"),
            new GameReviewListViewModel(2,"Игра 2"),
            new GameReviewListViewModel(3,"Игра 3"),
            new GameReviewListViewModel(4,"Игра 4"),
            new GameReviewListViewModel(5,"Игра 5"),
        });
    }
}
