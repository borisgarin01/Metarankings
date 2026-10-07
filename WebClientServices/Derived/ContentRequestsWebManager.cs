using Domain.ContentRequests;
using Domain.RequestsModels.ContentRequests;
using System.Net.Http.Json;

namespace WebManagers.Derived;

/// <summary>
/// Клиент API заявок на добавление игр и фильмов.
/// </summary>
public sealed class ContentRequestsWebManager : WebManager
{
    private const string BasePath = "/api/ContentRequests";

    public ContentRequestsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    /// <summary>
    /// Все заявки, только для администраторов.
    /// </summary>
    public async Task<IEnumerable<ContentRequest>> GetAllAsync(ContentRequestStatus? status = null)
    {
        string url = status is null ? BasePath : $"{BasePath}?status={(short)status.Value}";

        IEnumerable<ContentRequest>? contentRequests = await Client.GetFromJsonAsync<IEnumerable<ContentRequest>>(url);

        return contentRequests ?? Enumerable.Empty<ContentRequest>();
    }

    /// <summary>
    /// Заявки текущего пользователя.
    /// </summary>
    public async Task<IEnumerable<ContentRequest>> GetMineAsync()
    {
        IEnumerable<ContentRequest>? contentRequests = await Client.GetFromJsonAsync<IEnumerable<ContentRequest>>($"{BasePath}/my");

        return contentRequests ?? Enumerable.Empty<ContentRequest>();
    }

    public async Task<HttpResponseMessage> AddAsync(AddContentRequestModel addContentRequestModel)
    {
        return await Client.PostAsJsonAsync(BasePath, addContentRequestModel);
    }

    public async Task<HttpResponseMessage> UpdateStatusAsync(long id, UpdateContentRequestStatusModel updateContentRequestStatusModel)
    {
        return await Client.PutAsJsonAsync($"{BasePath}/{id}/status", updateContentRequestStatusModel);
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        return await Client.DeleteAsync($"{BasePath}/{id}");
    }
}
