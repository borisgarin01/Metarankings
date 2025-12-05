using Domain.Games;
using Domain.RequestsModels.Games;

namespace Data.Repositories.Interfaces.Derived;

public interface IGamesRepository : IRepository<Game, AddGameModel, UpdateGameModel>
{
    public Task<IEnumerable<Game>> GetByParametersAsync(long[]? genresIds, long[]? platformsIds, long[]? years, long[]? developersIds, long[]? publishersIds, int skip, int take);
}
