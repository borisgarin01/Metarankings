using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesGenresController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IMoviesGenresRepository moviesGenresRepository;

    public MoviesGenresController(IConfiguration configuration, IMoviesGenresRepository moviesGenresRepository)
    {
        this.configuration = configuration;
        this.moviesGenresRepository = moviesGenresRepository;
    }

    [HttpPost]
    public async Task<ActionResult> AddGenreAsync(Genre genre)
    {
        await moviesGenresRepository.AddAsync(genre);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> MoviesGenres()
    {
        var moviesGenres = await moviesGenresRepository.GetAllAsync();
        return Ok(moviesGenres);
    }
}
