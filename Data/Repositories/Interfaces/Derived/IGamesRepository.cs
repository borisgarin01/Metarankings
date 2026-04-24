using Domain.Games;
using Domain.RequestsModels.Games;

namespace Data.Repositories.Interfaces.Derived;

public interface IGamesRepository : IRepository<Game, AddGameModel, UpdateGameModel>
{
    public Task<IEnumerable<Game>> GetByParametersAsync(long[]? genresIds, long[]? platformsIds, int[]? years, long[]? developersIds, long[]? publishersIds, int skip, int take);
    Task<IEnumerable<Game>> GetFirstAsync(int offset, int limit);
    Task<IEnumerable<Game>> GetLastAsync(int offset, int limit);
    Task<IEnumerable<Game>> GetNearestAsync(short limit);
}
