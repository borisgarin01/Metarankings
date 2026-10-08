using Domain.RequestsModels.CriticsReviews;
using Domain.Reviews;
using System.Net.Http.Json;

namespace WebManagers.Derived.CriticsReviews;

/// <summary>
/// Клиент API рецензий критиков.
/// </summary>
public abstract class CriticsReviewsWebManager : WebManager
{
    private readonly string _basePath;

    protected CriticsReviewsWebManager(IHttpClientFactory httpClientFactory, string basePath) : base(httpClientFactory)
    {
        _basePath = basePath;
    }

    public async Task<IEnumerable<CriticReview>> GetByEntityAsync(long entityId, CancellationToken cancellationToken = default)
    {
        IEnumerable<CriticReview>? criticsReviews = await Client.GetFromJsonAsync<IEnumerable<CriticReview>>($"{_basePath}/entity/{entityId}", cancellationToken);

        return criticsReviews ?? Enumerable.Empty<CriticReview>();
    }

    public async Task<HttpResponseMessage> AddAsync(CriticReviewModel criticReviewModel, CancellationToken cancellationToken = default)
    {
        return await Client.PostAsJsonAsync(_basePath, criticReviewModel, cancellationToken);
    }

    public async Task<HttpResponseMessage> UpdateAsync(long id, CriticReviewModel criticReviewModel, CancellationToken cancellationToken = default)
    {
        return await Client.PutAsJsonAsync($"{_basePath}/{id}", criticReviewModel, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Client.DeleteAsync($"{_basePath}/{id}", cancellationToken);
    }
}
