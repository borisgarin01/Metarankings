using Domain.Games;
using Domain.RequestsModels.Games;
using System.Net.Http.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesWebManager : CrudWebManager<Game, AddGameModel, UpdateGameModel>, IByNameSearchingManager<Game>
{
    public GamesWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Games")
    {
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameModel> addGamesModels, CancellationToken cancellationToken = default)
    {
        return PostAsync("upload-games-from-json", addGamesModels, cancellationToken);
    }

    public override async Task<IEnumerable<Game>> GetFirstAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Game>>($"{BasePath}/First/{offset}/{limit}", cancellationToken);
    }

    public override async Task<IEnumerable<Game>> GetLastAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Game>>($"{BasePath}/Last/{offset}/{limit}", cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetNearestAsync(
        long offset,
        long limit,
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null,
        CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Game>>(
            $"{BasePath}/games-releases-dates/{offset}/{limit}{BuildFiltersQuery(genresIds, platformsIds)}", cancellationToken);
    }

    public async Task<int> GetNearestCountAsync(
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null,
        CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<int>(
            $"{BasePath}/games-releases-dates/count{BuildFiltersQuery(genresIds, platformsIds)}", cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetMostWaitingAsync(
        int offset,
        int limit,
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Game>? games = await Client.GetFromJsonAsync<IEnumerable<Game>>(
            $"{BasePath}/most-waiting/{offset}/{limit}{BuildFiltersQuery(genresIds, platformsIds)}", cancellationToken);

        return games ?? Enumerable.Empty<Game>();
    }

    public async Task<int> GetMostWaitingCountAsync(
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null,
        CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<int>(
            $"{BasePath}/most-waiting/count{BuildFiltersQuery(genresIds, platformsIds)}", cancellationToken);
    }

    public async Task<IEnumerable<Game>> SearchByName(string? name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetAllAsync(cancellationToken);

        return await Client.GetFromJsonAsync<IEnumerable<Game>>($"{BasePath}/Search?name={name}", cancellationToken);
    }

    private static string BuildFiltersQuery(IEnumerable<long>? genresIds, IEnumerable<long>? platformsIds)
    {
        return QueryStringBuilder.Build(("genresIds", genresIds), ("platformsIds", platformsIds));
    }
}
