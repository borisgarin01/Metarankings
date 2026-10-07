using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;
using System.Net.Http.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesWebManager : CrudWebManager<Movie, AddMovieModel, UpdateMovieModel>, IByNameSearchingManager<Movie>
{
    public MoviesWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/movies")
    {
    }

    public async Task<IEnumerable<Movie>> GetMostWaitingAsync(
        int offset,
        int limit,
        IEnumerable<long>? genresIds = null)
    {
        IEnumerable<Movie>? movies = await Client.GetFromJsonAsync<IEnumerable<Movie>>(
            $"{BasePath}/most-waiting/{offset}/{limit}{BuildGenresQuery(genresIds)}");

        return movies ?? Enumerable.Empty<Movie>();
    }

    public async Task<int> GetMostWaitingCountAsync(IEnumerable<long>? genresIds = null)
    {
        return await Client.GetFromJsonAsync<int>($"{BasePath}/most-waiting/count{BuildGenresQuery(genresIds)}");
    }

    public async Task<IEnumerable<Movie>> SearchByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetAllAsync();

        return await Client.GetFromJsonAsync<IEnumerable<Movie>>($"{BasePath}/Search?name={name}");
    }

    private static string BuildGenresQuery(IEnumerable<long>? genresIds)
    {
        return QueryStringBuilder.Build(("genresIds", genresIds));
    }
}
