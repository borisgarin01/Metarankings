using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IGenresRepository genresRepository;

    public GenresController(IConfiguration configuration, IGenresRepository genresRepository)
    {
        this.configuration = configuration;
        this.genresRepository = genresRepository;
    }

    [HttpPost]
    public async Task<ActionResult> AddGenreAsync(Genre genre)
    {
        await genresRepository.AddAsync(genre);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> Genres()
    {
        var genres = await genresRepository.GetAllAsync();
        return Ok(genres);
    }
}
