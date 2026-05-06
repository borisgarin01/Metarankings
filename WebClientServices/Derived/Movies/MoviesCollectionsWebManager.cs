using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesCollectionsWebManager : WebManager, IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>
{
    public MoviesCollectionsWebManager(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMoviesCollectionModel addGameCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Movies/Collections", addGameCollectionModel);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMoviesCollectionModel> adds)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> DeleteAsync(long id)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Movies/Collections/{id}");
        return httpResponseMessage;
    }

    public async Task<IEnumerable<MoviesCollection>> GetAllAsync()
    {
        var moviesCollections = await HttpClient.GetFromJsonAsync<IEnumerable<MoviesCollection>>($"/api/Movies/Collections");
        return moviesCollections;
    }

    public async Task<IEnumerable<MoviesCollection>> GetFirstAsync(long offset, long limit)
    {
        var moviesCollections = await HttpClient.GetFromJsonAsync<IEnumerable<MoviesCollection>>($"/api/Movies/Collections/{offset}/{limit}");
        return moviesCollections;
    }

    public async Task<MoviesCollection> GetAsync(long id)
    {
        var moviesCollection = await HttpClient.GetFromJsonAsync<MoviesCollection>($"/api/Movies/Collections/{id}");
        return moviesCollection;
    }

    public async Task<MoviesCollection> UpdateAsync(long id, UpdateMoviesCollectionModel updateMovieCollectionModel)
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync<UpdateMoviesCollectionModel>($"/api/Movies/Collections/{id}", updateMovieCollectionModel);

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
        {
            var updatedMoviesCollection = await JsonSerializer.DeserializeAsync<MoviesCollection>(await httpResponseMessage.Content.ReadAsStreamAsync());
            return updatedMoviesCollection;
        }

        return null;
    }

    public Task<IEnumerable<MoviesCollection>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }
}
