using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using WebManagers;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearByGenresComponent : ComponentBase
{
    [CascadingParameter(Name = "Year")]
    public int? Year { get; set; }

    [CascadingParameter(Name = "GenreId")]
    public long? GenreId { get; set; }

    [CascadingParameter(Name = "PlatformId")]
    public long? PlatformId { get; set; }

    private IEnumerable<Genre> Genres { get; set; } = new List<Genre>();

    [Inject]
    private IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GenresWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genres = await GenresWebManager.GetAllAsync();
    }

    private string BuildUrl(long? genreId = null)
    {
        var parameters = new List<string>();

        if (Year.HasValue) parameters.Add($"Year={Year}");
        if (PlatformId.HasValue) parameters.Add($"PlatformId={PlatformId}");
        if (genreId.HasValue) parameters.Add($"GenreId={genreId}");

        return parameters.Any() ? $"/games/best-games/?{string.Join("&", parameters)}" : "/games/best-games/";
    }
}
