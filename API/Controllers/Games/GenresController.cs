using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class GenresController : CrudControllerBase<Genre, AddGameGenreModel, UpdateGameGenreModel>
{
    public GenresController(IRepository<Genre, AddGameGenreModel, UpdateGameGenreModel> genresRepository) : base(genresRepository)
    {
    }
}
