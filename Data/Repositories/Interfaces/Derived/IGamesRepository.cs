using Domain.Games;
using Domain.RequestsModels.Games;

namespace Data.Repositories.Interfaces.Derived;

public interface IGamesRepository : IRepository<Game, AddGameModel, UpdateGameModel>
{
    Task<IEnumerable<Game>> GetByParametersAsync(
        long[]? genresIds,
        long[]? platformsIds,
        int[]? years,
        long[]? developersIds,
        long[]? publishersIds,
        long[]? localizationIds,
        int skip,
        int take
    );

    Task<int> GetCountByParametersAsync(
        long[]? genresIds,
        long[]? platformsIds,
        int[]? years,
        long[]? developersIds,
        long[]? publishersIds,
        long[]? localizationIds
    );

    Task<IEnumerable<Game>> GetFirstAsync(int offset, int limit);

    Task<IEnumerable<Game>> GetLastAsync(int offset, int limit);
    Task<IEnumerable<Game>> GetNearestAsync(short offset, short limit);
    Task<IEnumerable<Game>> GetNearestAsync();

    Task<IEnumerable<Game>> GetNearestByParametersAsync(
    long[]? genresIds,
    long[]? platformsIds,
    short offset,
    short limit);

    Task<int> GetNearestCountByParametersAsync(

        long[]? genresIds,
        long[]? platformsIds);

    Task<IEnumerable<Game>> GetByNameAsync(string name);

    Task<IEnumerable<Game>> GetMostWaitingAsync(long[]? genresIds, long[]? platformsIds, int skip, int take);

    Task<int> GetMostWaitingCountAsync(long[]? genresIds, long[]? platformsIds);
}
