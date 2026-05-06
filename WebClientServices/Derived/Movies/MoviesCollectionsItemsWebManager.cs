using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesCollectionsItemsWebManager : WebManager, IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>
{
    public MoviesCollectionsItemsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMoviesCollectionItemModel addMoviesCollectionItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Movies/CollectionsItems", addMoviesCollectionItemModel);
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
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Movies/CollectionsItems/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<MoviesCollectionItem>> GetAllAsync()
    {
        var moviesCollectionsItems = await HttpClient.GetFromJsonAsync<IEnumerable<MoviesCollectionItem>>($"/api/Movies/CollectionsItems");
        return moviesCollectionsItems;
    }

    public async Task<MoviesCollectionItem> GetAsync(long id)
    {
        var moviesCollectionsItem = await HttpClient.GetFromJsonAsync<MoviesCollectionItem>($"/api/Movies/CollectionsItems/{id}");
        return moviesCollectionsItem;
    }

    public async Task<IEnumerable<MoviesCollectionItem>> GetFirstAsync(long offset, long limit)
    {
        var moviesCollectionsItems = await HttpClient.GetFromJsonAsync<IEnumerable<MoviesCollectionItem>>($"/api/Movies/CollectionsItems/{offset}/{limit}");
        return moviesCollectionsItems;
    }

    public Task<IEnumerable<MoviesCollectionItem>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public async Task<MoviesCollectionItem> UpdateAsync(long id, UpdateMoviesCollectionItemModel updateMoviesCollectionItemModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateMoviesCollectionItemModel>($"/api/Movies/CollectionsItems/{id}", updateMoviesCollectionItemModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            var updatedMovieCollectionItem = await JsonSerializer.DeserializeAsync<MoviesCollectionItem>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedMovieCollectionItem;
        }

        return null;
    }
}
