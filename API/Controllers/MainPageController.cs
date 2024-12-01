using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MainPageController : ControllerBase
{
    [HttpGet("movies")]
    public async Task<ActionResult<IEnumerable<MoviesIndexComponentItem>>> GetIndexMoviesComponentsItemsGenresAsync()
    {
        var indexMoviesComponentsItemsGenres = new MoviesIndexComponentItem[]
        {
            new MoviesIndexComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/comedy/", Name="Комедии" },
                    new Genre { Href="https://metarankings.ru/meta/movies/novogodnie/",Name="Новогодние" },
                    new Genre { Href="https://metarankings.ru/meta/movies/horror/", Name="Ужасы" },
                    new Genre { Href="https://metarankings.ru/meta/movies/fantasy-movies/", Name="Фэнтези" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=5.6f,
                    Title="Дорогой Санта",
                    ImageAlt="Дорогой Санта",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/dorogoj-santa-2024-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/dorogoj-santa-2024/",
                    Description="Когда юный мальчик отправляет свой рождественский список желаний Санте с одной важной орфографической ошибкой, появляется дьявольский Джек Блэк, чтобы устроить хаос на празднике",
                    ReleaseDate=new DateTime(2024,11,25)
                }
            },
            new MoviesIndexComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/comedy/", Name="Комедии" },
                    new Genre { Href="https://metarankings.ru/meta/movies/semejnye/",Name="Семейные" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score = 6.3f,
                    Title = "Приключения Паддингтона 3",
                    ImageAlt = "Приключения Паддингтона 3",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/priklyucheniya-paddingtona-3-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref = "https://metarankings.ru/priklyucheniya-paddingtona-3/",
                    Description = "Фильм продолжает историю знаменитого медвежонка. На этот раз Паддингтон и семья Браунов решают навестить тетю Люси в Перу, но загадочное событие заставляет их",
                    ReleaseDate = new DateTime(2024,11,28)
                }
            },
            new MoviesIndexComponentItem
            {
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/thrillers/", Name="Триллеры" },
                    new Genre { Href="https://metarankings.ru/meta/movies/horror/",Name="Ужасы" }
                },
                IndexComponentItem = new IndexComponentItem
                {
                    Score =5.7f,
                    Title="Перевозчик душ",
                    ImageAlt="Перевозчик душ",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/perevozchik-dush-2024-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/perevozchik-dush-2024/",
                    Description="После ссоры, Энн и Патрик, едва сдерживая взаимное раздражение, хватают первое попавшееся такси. Водитель, изначально казавшийся добродушным и общительным, вдруг начинает вести себя...",
                    ReleaseDate=new DateTime(2024,11,25)
                }
            },
            new MoviesIndexComponentItem
            {
                IndexComponentItem = new IndexComponentItem
                {

                    Score = 4.9f,
                    Title = "Эммануэль",
                    ImageAlt = "Эммануэль",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/emmanuel-2024-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/emmanuel-2024/",
                    Description = "Роскошный гонконгский отель становится местом, где Эммануэль, находящаяся в деловой поездке, исследует свою сексуальность и переосмысливает понятия близости и свободы, проверяя границы дозволенного...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                },
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" },
                    new Genre { Href="https://metarankings.ru/meta/movies/melodramas/",Name="Мелодрамы" }
                }
            },
            new MoviesIndexComponentItem
            {
                IndexComponentItem = new  IndexComponentItem
                {
                    Score = 0f,
                    Title = "Последнее замыкание. Конец света",
                    ImageAlt = "Последнее замыкание. Конец света",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/poslednee-zamykanie-konec-sveta-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/poslednee-zamykanie-konec-sveta-2024/",
                    Description = "В мире будущего, где человечество проиграло климатические войны, выжила, возможно, только Ева. Она живет в лагере, который охраняет робот, не пуская никого без...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                },
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" },
                    new Genre { Href="https://metarankings.ru/meta/movies/fantasy/",Name="Фантастика" }
                }
            },
            new MoviesIndexComponentItem
            {
                IndexComponentItem= new IndexComponentItem
                {

                    Score = 5.7f,
                    Title = "Мария",
                    ImageAlt = "Мария",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/mariya-2024-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/mariya-2024/",
                    Description = "В своей роскошной парижской квартире, в 1970-х, легендарная Мария Каллас переживает последние годы жизни, сражаясь с внутренними демонами. Сентябрь 1977 года: окружённая пуделями,...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                },
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" }
                }
            },
            new MoviesIndexComponentItem
            {
                IndexComponentItem = new IndexComponentItem
                {
                    Score = 0f,
                    Title = "Астрал. Медиум",
                    ImageAlt = "Астрал. Медиум",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/astral-medium-2024-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/astral-medium-2024/",
                    Description = "Слепая медиум Дарси, способная видеть прошлое, приезжает в дом, где год назад была зверски убита ее сестра-близнец Дани. Сейчас в этом доме живет...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                },
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/horror/", Name="Ужасы" }
                }
            },
            new MoviesIndexComponentItem
            {
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/thrillers/", Name="Триллеры" }
                },
                IndexComponentItem = new IndexComponentItem
                {
                    Score = 0f,
                    Title = "Я слежу за тобой",
                    ImageAlt = "Я слежу за тобой",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/ya-slezhu-za-toboj-2024-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/ya-slezhu-za-toboj-2024/",
                    Description = "У риелтора Ку Джон-тхэ есть секрет: он тайно проникает в дома клиентов, подглядывая за их жизнью. Однажды, во время очередного вторжения, он находит...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                }
            },
            new MoviesIndexComponentItem
            {
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/comedy/", Name="Комедии" }
                },
                IndexComponentItem=  new IndexComponentItem
                {
                    Score = 0f,
                    Title = "Один день в Стамбуле",
                    ImageAlt = "Один день в Стамбуле",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/odin-den-v-stambule-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/odin-den-v-stambule-2024/",
                    Description = "Смерть друга Вадима разрушила планы друзей на отдых на яхте в Стамбуле. Однако, они проводят прощальную прогулку по Босфору, делясь воспоминаниями о нём...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                }
            },
            new MoviesIndexComponentItem
            {
                Genres = new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" }
                },
                IndexComponentItem = new IndexComponentItem
                {
                    Score = 7.6f,
                    Title = "Птица",
                    ImageAlt = "Птица",
                    ImageSrc = "https://metarankings.ru/images/uploads/2024/11/ptica-2024-cover-art-50x70.jpg",
                    Id = 1,
                    ItemHref = "https://metarankings.ru/ptica-2024/",
                    Description = "Бэйли и ее брат Хантер живут в заброшенном доме на севере Кента с отцом, Багом, который воспитывает их один и не всегда может...",
                    ReleaseDate = new DateTime(2024, 11, 28)
                }
            }
        };
        return Ok(indexMoviesComponentsItemsGenres);
    }

    [HttpGet("games")]
    public async Task<ActionResult<IEnumerable<GamesIndexComponentItem>>> GetIndexGamesComponentsItemsGenresAsync()
    {
        var indexGamesComponentsItemsGenres = new GamesIndexComponentItem[]
        {
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-series-x/",Name="Xbox Series X" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=6.5f,
                    Title="S.T.A.L.K.E.R. 2: Heart of Chernobyl",
                    ImageAlt="S.T.A.L.K.E.R. 2: Heart of Chernobyl",
                    ImageSrc="https://metarankings.ru/images/uploads/2022/04/stalker-2-heart-of-chernobyl-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/stalker-2/",
                    Description="Игра S.T.A.L.K.E.R. 2: Heart of Chernobyl — продолжение культовой серии ролевых шутеров с видом от первого лица в открытом мире постапокалиптической зоны отчуждения Чернобыльской...",
                    ReleaseDate=new DateTime(2024,11,20)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/",Name="PS5" },
                    new Platform { Href="https://metarankings.ru/meta/games/switch/",Name="Switch" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=5.1f,
                    Title="LEGO Horizon Adventures",
                    ImageAlt="LEGO Horizon Adventures",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/lego-horizon-adventures-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/lego-horizon-adventures/",
                    Description="Присоединяйтесь к Элой, отважной охотнице, и отправляйтесь в захватывающее приключение, чтобы спасти мир от Хелиса, злодея, возглавляющего банду солнцепоклонников, поклоняющихся таинственному Древнему Злу...",
                    ReleaseDate=new DateTime(2024,11,14)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/",Name="PS5" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-series-x/",Name="Xbox Series X" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=6.4f,
                    Title="Farming Simulator 25",
                    ImageAlt="Farming Simulator 25",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/farming-simulator-25-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/farming-simulator-25/",
                    Description="Погрузитесь в мир увлекательной сельской жизни! Создайте ферму своей мечты в одиночку или вместе с друзьями в кооперативном режиме. Выбирайте из потрясающих локаций:...",
                    ReleaseDate=new DateTime(2024,11,12)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps4/", Name="PS4" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/",Name="PS5" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-one/",Name="Xbox One" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-series-x/",Name="Xbox Series X" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=5.5f,
                    Title="Slitterhead",
                    ImageAlt="Slitterhead",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/slitterhead-boxart-cover-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/slitterhead/",
                    Description="В этой боевой приключенческой игре вы окажетесь на хаотичных, заваленных мусором улицах Коулонга, где царит атмосфера тайны и опасности. Вы &#8212; Хёки, существо...",
                    ReleaseDate=new DateTime(2024,11,8)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/switch/", Name="Switch" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=7f,
                    Title="Mario &#038; Luigi: Brothership",
                    ImageAlt="Mario &#038; Luigi: Brothership",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/mario-luigi-brothership-boxart-cover-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/mario-luigi-brothership/",
                    Description="В Mario &amp; Luigi: Brothership Братья Марио отправляются в новое морское приключение, чтобы спасти мир Конкордии! После того, как Уни-дерево разрушено, мир развалился...",
                    ReleaseDate=new DateTime(2024,11,7)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/", Name="PS5" },
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=7f,
                    Title="Metro Awakening",
                    ImageAlt="Metro Awakening",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/10/metro-awakening-boxart-cover-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/metro-awakening/",
                    Description="Metro Awakening &#8212; это VR-игра во вселенной Metro, которая перенесет вас в 2028 год. Мир после ядерной катастрофы. Выжившие люди прячутся в подземных...",
                    ReleaseDate=new DateTime(2024,11,7)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/switch/", Name="Switch" },
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=7f,
                    Title="Teenage Mutant Ninja Turtles: Splintered Fate",
                    ImageAlt="Teenage Mutant Ninja Turtles: Splintered Fate",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/teenage-mutant-ninja-turtles-splintered-fate-cover-art-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/teenage-mutant-ninja-turtles-splintered-fate/",
                    Description="В Teenage Mutant Ninja Turtles: Splintered Fate вас ждут динамичные и захватывающие бои в стиле roguelike, где каждый раз вас ждет новый вызов!...",
                    ReleaseDate=new DateTime(2024,11,6)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps4/", Name="PS4" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/", Name="PS5" },
                    new Platform { Href="https://metarankings.ru/meta/games/switch/", Name="Switch" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-one/", Name="Xbox One" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-series-x/", Name="Xbox Series X" },
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=6.9f,
                    Title="Metal Slug Tactics",
                    ImageAlt="Metal Slug Tactics",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/11/metal-slug-tactics-boxart-cover-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/metal-slug-tactics/",
                    Description="Metal Slug Tactics &#8212; это тактическая игра с пошаговыми боями, которая дает возможность управлять ходом битвы с помощью особых атак. Зарабатывайте опыт, открывайте...",
                    ReleaseDate=new DateTime(2024,11,5)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/", Name="PS5" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=6.3f,
                    Title="Horizon Zero Dawn Remastered",
                    ImageAlt="Horizon Zero Dawn Remastered",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/10/horizon-zero-dawn-remastered-boxart-cover-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/horizon-zero-dawn-remastered/",
                    Description="Horizon Zero Dawn — это приключенческая ролевая игра, завоевавшая множество наград и признание критиков. В версии Remastered, знакомые игрокам дикие земли оживают с...",
                    ReleaseDate=new DateTime(2024,10,31)
                }
            },
            new GamesIndexComponentItem
            {
                Platforms=new Platform[]
                {
                    new Platform { Href="https://metarankings.ru/meta/games/pc/", Name="PC" },
                    new Platform { Href="https://metarankings.ru/meta/games/ps5/", Name="PS5" },
                    new Platform { Href="https://metarankings.ru/meta/games/xbox-series-x/", Name="Xbox Series X" }
                },
                IndexComponentItem=new IndexComponentItem
                {
                    Score=6.3f,
                    Title="Dragon Age: The Veilguard",
                    ImageAlt="Dragon Age: The Veilguard",
                    ImageSrc="https://metarankings.ru/images/uploads/2024/12/dragon-age-the-veilguard-boxart-cover-50x70.jpg",
                    Id=1,
                    ItemHref="https://metarankings.ru/dragon-age-the-veilguard/",
                    Description="Ролевая экшен приключенческая игра Dragon Age: The Veilguard отправляет в мир Тедаса, яркую страну суровой дикой природы, коварных лабиринтов и сверкающих городов. Вас...",
                    ReleaseDate=new DateTime(2024,10,31)
                }
            },
        };
        return Ok(indexGamesComponentsItemsGenres);
    }

    [HttpGet("collections")]
    public async Task<ActionResult<IEnumerable<CollectionsComponentItem>>> GetCollectionsComponentItems()
    {
        var collectionsComponentsItems = new CollectionsComponentItem[]
        {
            new CollectionsComponentItem
            {
                CategoryHref="https://metarankings.ru/meta/collections/films-games/",
                CategoryTitle="Контент",
                ImageAlt="Самые сложные игры",
                ImageSrc="https://metarankings.ru/images/uploads/2023/06/slozhnye-igry-445x250.jpg",
                ItemHref="https://metarankings.ru/samye-slozhnye-igry/",
                Title="Самые сложные игры",
                Id=1
            },
            new CollectionsComponentItem
            {
                CategoryHref="https://metarankings.ru/meta/collections/films-games/",
                CategoryTitle="Контент",
                ImageAlt="Лучшие фильмы про женщин",
                ImageSrc="https://metarankings.ru/images/uploads/2023/06/films-pro-zhenshhin-445x250.jpg",
                ItemHref="https://metarankings.ru/best-films-pro-zhenshhin/",
                Title="Лучшие фильмы про женщин",
                Id=2
            },
            new CollectionsComponentItem
            {
                CategoryHref="https://metarankings.ru/meta/collections/films-games/",
                CategoryTitle="Контент",
                ImageAlt="Фильмы про реальные события",
                ImageSrc="https://metarankings.ru/images/uploads/2023/06/pro-realnye-sobytiya-445x250.jpg",
                ItemHref="https://metarankings.ru/filmy-pro-realnye-sobytiya/",
                Title="Фильмы про реальные события",
                Id=3
            },
            new CollectionsComponentItem
            {
                CategoryHref="https://metarankings.ru/meta/collections/films-games/",
                CategoryTitle="Контент",
                ImageAlt="Лучшие фильмы про монстров",
                ImageSrc="https://metarankings.ru/images/uploads/2023/06/filmy-pro-monstrov-445x250.jpg",
                ItemHref="https://metarankings.ru/luchshie-filmy-pro-monstrov/",
                Title="Лучшие фильмы про монстров",
                Id=4
            },
            new CollectionsComponentItem
            {
                CategoryHref="https://metarankings.ru/meta/collections/films-games/",
                CategoryTitle="Контент",
                ImageAlt="Самые страшные игры",
                ImageSrc="https://metarankings.ru/images/uploads/2023/06/strashnye-igry-445x250.jpg",
                ItemHref="https://metarankings.ru/meta/collections/films-games/",
                Title="Самые страшные игры",
                Id=5
            },
            new CollectionsComponentItem
            {
                CategoryHref="https://metarankings.ru/meta/collections/films-games/",
                CategoryTitle="Контент",
                ImageAlt="Лучшие игры с открытым миром",
                ImageSrc="https://metarankings.ru/images/uploads/2023/06/best-igry-s-otkrytym-mirom-445x250.jpg",
                ItemHref="https://metarankings.ru/best-games-open-world/",
                Title="Лучшие игры с открытым миром",
                Id=6
            }
        };
        return Ok(collectionsComponentsItems);
    }

    [HttpGet("gamesNewsComponentsItems")]
    public async Task<ActionResult<IEnumerable<GamesNewsComponentItem>>> GetGamesNewsComponentsItems()
    {
        var gamesNewsComponentsItems = new GamesNewsComponentItem[]
        {
            new GamesNewsComponentItem
            {
                ImageAlt="Sony объявила системные требования God of War Ragnarök для PC",
                ImageSrc="https://metarankings.ru/images/uploads/2024/08/god-of-war-ragnarok-445x250.jpg",
                LinkHref="https://metarankings.ru/sony-obyavila-sistemnye-trebovaniya-god-of-war-ragnarok-dlya-pc/",
                LinkTitle="Sony объявила системные требования God of War Ragnarök для PC"
            },
            new GamesNewsComponentItem
            {
                ImageAlt="Activision представила кинематографический трейлер зомби-режима Call of Duty: Black Ops 6",
                ImageSrc="https://metarankings.ru/images/uploads/2024/08/kinematograficheskij-trejler-zombi-rezhima-v-call-of-duty-black-ops-6-445x250.jpg",
                LinkHref="https://metarankings.ru/activision-predstavila-kinematograficheskij-trejler-zombi-rezhima-call-of-duty-black-ops-6/",
                LinkTitle="Activision представила кинематографический трейлер зомби-режима Call of Duty: Black Ops 6"
            },
            new GamesNewsComponentItem
            {
                ImageAlt="Nintendo показала Metroid Prime 4: Beyond для Switch",
                ImageSrc="https://metarankings.ru/images/uploads/2024/06/metroid-prime-4-beyond-445x250.jpg",
                LinkHref="https://metarankings.ru/nintendo-pokazala-metroid-prime-4-beyond-dlya-switch/",
                LinkTitle="Nintendo показала Metroid Prime 4: Beyond для Switch"
            },
        };

        return Ok(gamesNewsComponentsItems);
    }
}
