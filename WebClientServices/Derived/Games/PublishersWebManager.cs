using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Games;

public sealed class PublishersWebManager : WebManager, IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    public PublishersWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddPublisherModel addPublisherModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Publishers", addPublisherModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddPublisherModel> addPublishersModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Games/Publishers/upload-publishers-from-json", addPublishersModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Games/Publishers/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        IEnumerable<Publisher>? publishers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Publisher>>("/api/Games/Publishers");
        return publishers;
    }

    public async Task<IEnumerable<Publisher>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<Publisher>? publishers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Publisher>>($"/api/Games/Publishers/{offset}/{limit}");
        return publishers;
    }

    public async Task<Publisher> GetAsync(long id)
    {
        Publisher? publisher = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Publisher>($"/api/Games/Publishers/{id}");
        return publisher;
    }

    public async Task<Publisher> UpdateAsync(long id, UpdatePublisherModel updatePublisherModel)
    {
        HttpResponseMessage publisherUpdateHttpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/Games/Publishers/{id}", updatePublisherModel);
        if (publisherUpdateHttpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<Publisher>(await publisherUpdateHttpResponseMessage.Content.ReadAsStreamAsync());
        return null;
    }

    public Task<IEnumerable<Publisher>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
