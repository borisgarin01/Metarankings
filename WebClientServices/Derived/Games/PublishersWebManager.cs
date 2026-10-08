using Domain.Games;
using Domain.RequestsModels.Games.Publishers;

namespace WebManagers.Derived.Games;

public sealed class PublishersWebManager : CrudWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    public PublishersWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Publishers")
    {
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddPublisherModel> addPublishersModels, CancellationToken cancellationToken = default)
    {
        return PostAsync("upload-publishers-from-json", addPublishersModels, cancellationToken);
    }
}
