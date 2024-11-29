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
                Title="Дорогой Санта",
                ImageAlt="Дорогой Санта",
                ImageSrc="https://metarankings.ru/images/uploads/2024/11/dorogoj-santa-2024-cover-art-50x70.jpg",
                Id=1,
                ItemHref="https://metarankings.ru/dorogoj-santa-2024/"
            }
        };
        return Ok(indexMoviesComponentsItemsGenres);
    }
}
