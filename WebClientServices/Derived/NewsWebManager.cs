using Domain.Common.News;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived;

public sealed class NewsWebManager : WebManager, IWebManager<NewsItem, AddNewsItemModel, UpdateNewsItemModel>
{
    public NewsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddNewsItemModel addNewsItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/News", addNewsItemModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/News/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<NewsItem>> GetAllAsync()
    {
        IEnumerable<NewsItem>? news = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<NewsItem>>($"/api/News");
        return news;
    }

    public async Task<IEnumerable<NewsItem>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<NewsItem>? news = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<NewsItem>>($"/api/News?offset={offset}&limit={limit}");
        return news;
    }

    public async Task<NewsItem> GetAsync(long id)
    {
        NewsItem? news = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<NewsItem>($"/api/News/{id}");
        return news;
    }

    public async Task<NewsItem> UpdateAsync(long id, UpdateNewsItemModel tUpdate)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync($"/api/News/{id}", tUpdate);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<NewsItem>(await httpResponseMessage.Content.ReadAsStreamAsync());
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<NewsItem>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddNewsItemModel> adds)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }
}
