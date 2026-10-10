using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;

namespace API.Controllers.Movies;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Movies })]
[Route("api/movies/[controller]")]
public sealed class MoviesStudiosController : CrudControllerBase<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    public MoviesStudiosController(IRepository<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> moviesStudiosRepository) : base(moviesStudiosRepository)
    {
    }

    // Маршрут [HttpGet("{id:long}")] наследуется от базового метода.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public override Task<ActionResult<MovieStudio>> GetAsync(long id)
    {
        return base.GetAsync(id);
    }
}
