using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using Microsoft.AspNetCore.Http;

namespace WebManagers.Derived.Games;

public sealed class GenresWebManager : CrudWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel>
{
    public GenresWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "/api/Games/Genres")
    {
    }

    public override Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        return PostAsync("genres-excel-upload", formFile);
    }

    public override Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGameGenreModel> addGenresModels)
    {
        return PostAsync("upload-genres-from-json", addGenresModels);
    }
}
