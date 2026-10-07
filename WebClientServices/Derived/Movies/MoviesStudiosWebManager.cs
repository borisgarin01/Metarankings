using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using System.Net.Http.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesStudiosWebManager : CrudWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>
{
    public MoviesStudiosWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Movies/MoviesStudios")
    {
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieStudioModel> addMoviesStudiosModels)
    {
        return Client.PostAsJsonAsync(BasePath, addMoviesStudiosModels);
    }
}
