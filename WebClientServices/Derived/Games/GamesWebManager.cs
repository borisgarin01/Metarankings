using Domain.Games;
using Domain.RequestsModels.Games;
using System.Net.Http.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesWebManager : CrudWebManager<Game, AddGameModel, UpdateGameModel>, IByNameSearchingManager<Game>
{
    public GamesWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Games")
    {
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameModel> addGamesModels)
    {
        return PostAsync("upload-games-from-json", addGamesModels);
    }

    public override async Task<IEnumerable<Game>> GetFirstAsync(long offset, long limit)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Game>>($"{BasePath}/First/{offset}/{limit}");
    }

    public override async Task<IEnumerable<Game>> GetLastAsync(long offset, long limit)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Game>>($"{BasePath}/Last/{offset}/{limit}");
    }

    public async Task<IEnumerable<Game>> GetNearestAsync(
        long offset,
        long limit,
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        return await Client.GetFromJsonAsync<IEnumerable<Game>>(
            $"{BasePath}/games-releases-dates/{offset}/{limit}{BuildFiltersQuery(genresIds, platformsIds)}");
    }

    public async Task<int> GetNearestCountAsync(
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        return await Client.GetFromJsonAsync<int>(
            $"{BasePath}/games-releases-dates/count{BuildFiltersQuery(genresIds, platformsIds)}");
    }

    public async Task<IEnumerable<Game>> GetMostWaitingAsync(
        int offset,
        int limit,
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        IEnumerable<Game>? games = await Client.GetFromJsonAsync<IEnumerable<Game>>(
            $"{BasePath}/most-waiting/{offset}/{limit}{BuildFiltersQuery(genresIds, platformsIds)}");

        return games ?? Enumerable.Empty<Game>();
    }

    public async Task<int> GetMostWaitingCountAsync(
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        return await Client.GetFromJsonAsync<int>(
            $"{BasePath}/most-waiting/count{BuildFiltersQuery(genresIds, platformsIds)}");
    }

    public async Task<IEnumerable<Game>> SearchByName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetAllAsync();

        return await Client.GetFromJsonAsync<IEnumerable<Game>>($"{BasePath}/Search?name={name}");
    }

    private static string BuildFiltersQuery(IEnumerable<long>? genresIds, IEnumerable<long>? platformsIds)
    {
        return QueryStringBuilder.Build(("genresIds", genresIds), ("platformsIds", platformsIds));
    }
}
