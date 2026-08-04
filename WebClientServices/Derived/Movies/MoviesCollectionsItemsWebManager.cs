using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesCollectionsItemsWebManager : WebManager, IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>
{
    public MoviesCollectionsItemsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMoviesCollectionItemModel addMoviesCollectionItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/Movies/CollectionsItems", addMoviesCollectionItemModel);
        return httpResponseMessage;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMoviesCollectionItemModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/Movies/CollectionsItems/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<MoviesCollectionItem>> GetAllAsync()
    {
        IEnumerable<MoviesCollectionItem>? moviesCollectionsItems = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<MoviesCollectionItem>>($"/api/Movies/CollectionsItems");
        return moviesCollectionsItems;
    }

    public async Task<MoviesCollectionItem> GetAsync(long id)
    {
        MoviesCollectionItem? moviesCollectionsItem = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<MoviesCollectionItem>($"/api/Movies/CollectionsItems/{id}");
        return moviesCollectionsItem;
    }

    public async Task<IEnumerable<MoviesCollectionItem>> GetFirstAsync(long offset, long limit)
    {
        IEnumerable<MoviesCollectionItem>? moviesCollectionsItems = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<MoviesCollectionItem>>($"/api/Movies/CollectionsItems/{offset}/{limit}");
        return moviesCollectionsItems;
    }

    public Task<IEnumerable<MoviesCollectionItem>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task<MoviesCollectionItem> UpdateAsync(long id, UpdateMoviesCollectionItemModel updateMoviesCollectionItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").PutAsJsonAsync<UpdateMoviesCollectionItemModel>($"/api/Movies/CollectionsItems/{id}", updateMoviesCollectionItemModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            MoviesCollectionItem? updatedMovieCollectionItem = await JsonSerializer.DeserializeAsync<MoviesCollectionItem>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedMovieCollectionItem;
        }

        return null;
    }
}
