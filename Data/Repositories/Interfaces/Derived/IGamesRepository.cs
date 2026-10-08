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
        int take,
        CancellationToken cancellationToken = default
    );

    Task<int> GetCountByParametersAsync(
        long[]? genresIds,
        long[]? platformsIds,
        int[]? years,
        long[]? developersIds,
        long[]? publishersIds,
        long[]? localizationIds,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<Game>> GetFirstAsync(int offset, int limit, CancellationToken cancellationToken = default);

    Task<IEnumerable<Game>> GetLastAsync(int offset, int limit, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetNearestAsync(short offset, short limit, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetNearestAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Game>> GetNearestByParametersAsync(
    long[]? genresIds,
    long[]? platformsIds,
    short offset,
    short limit,
    CancellationToken cancellationToken = default);

    Task<int> GetNearestCountByParametersAsync(

        long[]? genresIds,
        long[]? platformsIds,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Game>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<Game>> GetMostWaitingAsync(long[]? genresIds, long[]? platformsIds, int skip, int take, CancellationToken cancellationToken = default);

    Task<int> GetMostWaitingCountAsync(long[]? genresIds, long[]? platformsIds, CancellationToken cancellationToken = default);
}
