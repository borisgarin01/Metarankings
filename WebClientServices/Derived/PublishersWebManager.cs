using Domain.Games;
using Domain.RequestsModels.Games.Publishers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class PublishersWebManager : WebManager, IWebManager<Publisher, AddPublisherModel, UpdatePublisherModel>
{
    public PublishersWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddPublisherModel addPublisherModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddPublisherModel>("/api/Publishers", addPublisherModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddPublisherModel> addPublishersModels)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Publishers/upload-publishers-from-json", addPublishersModels);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Publishers/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync()
    {
        var publishers = await HttpClient.GetFromJsonAsync<IEnumerable<Publisher>>("/api/Publishers");
        return publishers;
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync(long offset, long limit)
    {
        var publishers = await HttpClient.GetFromJsonAsync<IEnumerable<Publisher>>($"/api/Publishers/{offset}/{limit}");
        return publishers;
    }

    public async Task<Publisher> GetAsync(long id)
    {
        var publisher = await HttpClient.GetFromJsonAsync<Publisher>($"/api/Publishers/{id}");
        return publisher;
    }

    public async Task<Publisher> UpdateAsync(long id, UpdatePublisherModel updatePublisherModel)
    {
        HttpResponseMessage publisherUpdateHttpResponseMessage = await HttpClient.PutAsJsonAsync<UpdatePublisherModel>($"/api/Publishers/{id}", updatePublisherModel);
        if (publisherUpdateHttpResponseMessage.IsSuccessStatusCode)
            return await JsonSerializer.DeserializeAsync<Publisher>(await publisherUpdateHttpResponseMessage.Content.ReadAsStreamAsync());
        return null;
    }
}
