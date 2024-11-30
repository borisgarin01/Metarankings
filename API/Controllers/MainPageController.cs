using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MainPageController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IndexMoviesComponentItem>>> GetIndexMoviesComponentsItemsGenresAsync()
    {
        var indexMoviesComponentsItemsGenres = new IndexMoviesComponentItem[]
        {
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/comedy/", Name="Комедии" },
                    new Genre { Href="https://metarankings.ru/meta/movies/novogodnie/",Name="Новогодние" },
                    new Genre { Href="https://metarankings.ru/meta/movies/horror/", Name="Ужасы" },
                    new Genre { Href="https://metarankings.ru/meta/movies/fantasy-movies/", Name="Фэнтези" }
                },
                Score=5.6f,
                Title="Дорогой Санта",
                ImageAlt="Дорогой Санта",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/dorogoj-santa-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/dorogoj-santa-2024/",
                Description="Когда юный мальчик отправляет свой рождественский список желаний Санте с одной важной орфографической ошибкой, появляется дьявольский Джек Блэк, чтобы устроить хаос на празднике",
                ReleaseDate=new DateTime(2024,11,25)
            },
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/comedy/", Name="Комедии" },
                    new Genre { Href="https://metarankings.ru/meta/movies/semejnye/",Name="Семейные" }
                },
                Score=6.3f,
                Title="Приключения Паддингтона 3",
                ImageAlt="Приключения Паддингтона 3",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/priklyucheniya-paddingtona-3-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/priklyucheniya-paddingtona-3/",
                Description="Фильм продолжает историю знаменитого медвежонка. На этот раз Паддингтон и семья Браунов решают навестить тетю Люси в Перу, но загадочное событие заставляет их",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/thrillers/", Name="Триллеры" },
                    new Genre { Href="https://metarankings.ru/meta/movies/horror/",Name="Ужасы" }
                },
                Score=5.7f,
                Title="Перевозчик душ",
                ImageAlt="Перевозчик душ",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/perevozchik-dush-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/perevozchik-dush-2024/",
                Description="После ссоры, Энн и Патрик, едва сдерживая взаимное раздражение, хватают первое попавшееся такси. Водитель, изначально казавшийся добродушным и общительным, вдруг начинает вести себя...",
                ReleaseDate=new DateTime(2024,11,25)
            },
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" },
                    new Genre { Href="https://metarankings.ru/meta/movies/melodramas/",Name="Мелодрамы" }
                },
                Score=4.9f,
                Title="Эммануэль",
                ImageAlt="Эммануэль",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/emmanuel-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/emmanuel-2024/",
                Description="Роскошный гонконгский отель становится местом, где Эммануэль, находящаяся в деловой поездке, исследует свою сексуальность и переосмысливает понятия близости и свободы, проверяя границы дозволенного...",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Score=0f,
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" },
                    new Genre { Href="https://metarankings.ru/meta/movies/fantasy/",Name="Фантастика" }
                },
                Title="Последнее замыкание. Конец света",
                ImageAlt="Последнее замыкание. Конец света",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/poslednee-zamykanie-konec-sveta-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/poslednee-zamykanie-konec-sveta-2024/",
                Description="В мире будущего, где человечество проиграло климатические войны, выжила, возможно, только Ева. Она живет в лагере, который охраняет робот, не пуская никого без...",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" }
                },
                Score=5.7f,
                Title="Мария",
                ImageAlt="Мария",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/mariya-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/mariya-2024/",
                Description="В своей роскошной парижской квартире, в 1970-х, легендарная Мария Каллас переживает последние годы жизни, сражаясь с внутренними демонами. Сентябрь 1977 года: окружённая пуделями,...",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Score=0f,
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/horror/", Name="Ужасы" }
                },
                Title="Астрал. Медиум",
                ImageAlt="Астрал. Медиум",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/astral-medium-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/astral-medium-2024/",
                Description="Слепая медиум Дарси, способная видеть прошлое, приезжает в дом, где год назад была зверски убита ее сестра-близнец Дани. Сейчас в этом доме живет...",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Score=0f,
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/thrillers/", Name="Триллеры" }
                },
                Title="Я слежу за тобой",
                ImageAlt="Я слежу за тобой",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/ya-slezhu-za-toboj-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/ya-slezhu-za-toboj-2024/",
                Description="У риелтора Ку Джон-тхэ есть секрет: он тайно проникает в дома клиентов, подглядывая за их жизнью. Однажды, во время очередного вторжения, он находит...",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/comedy/", Name="Комедии" }
                },
                Score=0f,
                Title="Один день в Стамбуле",
                ImageAlt="Один день в Стамбуле",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/odin-den-v-stambule-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/odin-den-v-stambule-2024/",
                Description="Смерть друга Вадима разрушила планы друзей на отдых на яхте в Стамбуле. Однако, они проводят прощальную прогулку по Босфору, делясь воспоминаниями о нём...",
                ReleaseDate=new DateTime(2024,11,28)
            },
            new IndexMoviesComponentItem
            {
                Genres=new Genre[]
                {
                    new Genre { Href="https://metarankings.ru/meta/movies/dramy/", Name="Драмы" }
                },
                Score=7.6f,
                Title="Птица",
                ImageAlt="Птица",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/ptica-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/ptica-2024/",
                Description="Бэйли и ее брат Хантер живут в заброшенном доме на севере Кента с отцом, Багом, который воспитывает их один и не всегда может...",
                ReleaseDate=new DateTime(2024,11,28)
            }
        };
        return Ok(indexMoviesComponentsItemsGenres);
    }
}
