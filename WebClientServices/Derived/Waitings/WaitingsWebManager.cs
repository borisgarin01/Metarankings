using Domain.RequestsModels.Waitings;
using Domain.Waitings;
using System.Net.Http.Json;

namespace WebManagers.Derived.Waitings;

/// <summary>
/// Клиент API голосования "жду / не жду".
/// </summary>
public abstract class WaitingsWebManager : WebManager
{
    private readonly string _basePath;

    protected WaitingsWebManager(IHttpClientFactory httpClientFactory, string basePath) : base(httpClientFactory)
    {
        _basePath = basePath;
    }

    public async Task<WaitingStatistics?> GetAsync(long entityId)
    {
        return await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<WaitingStatistics>($"{_basePath}/{entityId}");
    }

    public async Task<IEnumerable<WaitingStatistics>> GetAsync(IEnumerable<long> entitiesIds)
    {
        if (!entitiesIds.Any())
            return Enumerable.Empty<WaitingStatistics>();

        string query = string.Join("&", entitiesIds.Distinct().Select(id => $"ids={id}"));

        IEnumerable<WaitingStatistics>? statistics = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<IEnumerable<WaitingStatistics>>($"{_basePath}?{query}");

        return statistics ?? Enumerable.Empty<WaitingStatistics>();
    }

    public async Task<HttpResponseMessage> VoteAsync(long entityId, bool isWaiting)
    {
        return await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .PostAsJsonAsync(_basePath, new AddWaitingVoteModel(entityId, isWaiting));
    }
}
