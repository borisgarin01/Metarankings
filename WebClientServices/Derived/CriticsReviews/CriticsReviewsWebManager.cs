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

    public async Task<IEnumerable<CriticReview>> GetByEntityAsync(long entityId)
    {
        IEnumerable<CriticReview>? criticsReviews = await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .GetFromJsonAsync<IEnumerable<CriticReview>>($"{_basePath}/entity/{entityId}");

        return criticsReviews ?? Enumerable.Empty<CriticReview>();
    }

    public async Task<HttpResponseMessage> AddAsync(CriticReviewModel criticReviewModel)
    {
        return await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .PostAsJsonAsync(_basePath, criticReviewModel);
    }

    public async Task<HttpResponseMessage> UpdateAsync(long id, CriticReviewModel criticReviewModel)
    {
        return await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .PutAsJsonAsync($"{_basePath}/{id}", criticReviewModel);
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        return await HttpClientFactory
            .CreateClient("AuthorizedClient")
            .DeleteAsync($"{_basePath}/{id}");
    }
}
