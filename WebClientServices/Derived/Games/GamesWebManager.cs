using Domain.Games;
using Domain.RequestsModels.Games;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesWebManager : WebManager, IWebManager<Game, AddGameModel, UpdateGameModel>, IByNameSearchingManager<Game>
{
    public GamesWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGameModel addGameModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Games", addGameModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameModel> addGamesModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/games/Games/upload-games-from-json", addGamesModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/games/Games/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        IEnumerable<Game>? games = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Game>>("/api/Games/Games");
        return games;
    }

    public async Task<IEnumerable<Game>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Game>? games = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/First/{offset}/{limit}");
        return games;
    }

    public async Task<IEnumerable<Game>> GetLastAsync(long offset, long limit)
    {
        IEnumerable<Game>? games = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/Last/{offset}/{limit}");
        return games;
    }

    public async Task<Game> GetAsync(long id)
    {
        Game? game = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Game>($"/api/Games/Games/{id}");
        return game;
    }

    public async Task<Game> UpdateAsync(long id, UpdateGameModel updateGameModel)
    {
        HttpResponseMessage publisherUpdateHttpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Games/Games/{id}", updateGameModel);
        if (publisherUpdateHttpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<Game>(await publisherUpdateHttpResponseMessage.Content.ReadAsStreamAsync());
        return null;
    }

    public async Task<IEnumerable<Game>> GetNearestAsync(
    long offset,
    long limit,
    IEnumerable<long>? genresIds = null,
    IEnumerable<long>? platformsIds = null)
    {
        var queryParams = new List<string>();

        if (genresIds?.Any() == true)
            queryParams.AddRange(genresIds.Select(id => $"genresIds={id}"));

        if (platformsIds?.Any() == true)
            queryParams.AddRange(platformsIds.Select(id => $"platformsIds={id}"));

        string query = queryParams.Count > 0
            ? "?" + string.Join("&", queryParams)
            : string.Empty;

        IEnumerable<Game> nearestGames = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<IEnumerable<Game>>(
                $"/api/Games/Games/games-releases-dates/{offset}/{limit}{query}");

        return nearestGames;
    }

    public async Task<int> GetNearestCountAsync(
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        var queryParams = new List<string>();

        if (genresIds?.Any() == true)
            queryParams.AddRange(genresIds.Select(id => $"genresIds={id}"));

        if (platformsIds?.Any() == true)
            queryParams.AddRange(platformsIds.Select(id => $"platformsIds={id}"));

        string query = queryParams.Count > 0
            ? "?" + string.Join("&", queryParams)
            : string.Empty;

        int count = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<int>(
                $"/api/Games/Games/games-releases-dates/count{query}");

        return count;
    }

    public async Task<IEnumerable<Game>> GetMostWaitingAsync(
        int offset,
        int limit,
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        IEnumerable<Game>? games = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<IEnumerable<Game>>(
                $"/api/Games/Games/most-waiting/{offset}/{limit}{BuildFiltersQuery(genresIds, platformsIds)}");

        return games ?? Enumerable.Empty<Game>();
    }

    public async Task<int> GetMostWaitingCountAsync(
        IEnumerable<long>? genresIds = null,
        IEnumerable<long>? platformsIds = null)
    {
        int count = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<int>(
                $"/api/Games/Games/most-waiting/count{BuildFiltersQuery(genresIds, platformsIds)}");

        return count;
    }

    private static string BuildFiltersQuery(IEnumerable<long>? genresIds, IEnumerable<long>? platformsIds)
    {
        List<string> queryParams = new List<string>();

        if (genresIds?.Any() == true)
            queryParams.AddRange(genresIds.Select(id => $"genresIds={id}"));

        if (platformsIds?.Any() == true)
            queryParams.AddRange(platformsIds.Select(id => $"platformsIds={id}"));

        return queryParams.Count > 0
            ? "?" + string.Join("&", queryParams)
            : string.Empty;
    }

    public async Task<IEnumerable<Game>> SearchByName(string? name)
    {
        IEnumerable<Game> games;
        if (!string.IsNullOrWhiteSpace(name))
            games = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/Games/Search?name={name}");
        else
            games = await GetAllAsync();
        return games;
    }
}
