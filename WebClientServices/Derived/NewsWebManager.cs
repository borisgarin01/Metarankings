using Domain.Common.News;
using System.Net.Http.Json;

namespace WebManagers.Derived;

public sealed class NewsWebManager : CrudWebManager<NewsItem, AddNewsItemModel, UpdateNewsItemModel>
{
    public NewsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/News")
    {
    }

    public override async Task<IEnumerable<NewsItem>> GetFirstAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        return await Client.GetFromJsonAsync<IEnumerable<NewsItem>>($"{BasePath}?offset={offset}&limit={limit}", cancellationToken);
    }
}
