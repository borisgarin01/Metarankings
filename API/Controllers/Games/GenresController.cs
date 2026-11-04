using Data.Repositories.Classes.Derived.Games;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;

namespace API.Controllers.Games;

[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.Games })]
[Route("api/games/[controller]")]
public sealed class GenresController : CrudControllerBase<Genre, AddGameGenreModel, UpdateGameGenreModel>
{
    private readonly GamesGenresRepository _gamesGenresRepository;

    public GenresController(GamesGenresRepository gamesGenresRepository) : base(gamesGenresRepository)
    {
        _gamesGenresRepository = gamesGenresRepository;
    }

    protected override async Task<ActionResult?> ValidateAddAsync(AddGameGenreModel addGameGenreModel)
    {
        Genre? existingGenre = await _gamesGenresRepository.GetByNameAsync(addGameGenreModel.Name);

        if (existingGenre is not null)
            return BadRequest($"Жанр игр с названием {addGameGenreModel.Name} уже существует");

        return null;
    }
}
