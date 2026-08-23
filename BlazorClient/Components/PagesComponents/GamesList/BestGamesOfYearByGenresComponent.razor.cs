namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearByGenresComponent : ComponentBase
{

    [CascadingParameter(Name = "Year")]
    public int? Year { get; set; }

    [CascadingParameter(Name = "GenreId")]
    public long? GenreId { get; set; }

    [CascadingParameter(Name = "PlatformId")]
    public long? PlatformId { get; set; }

    private string BuildUrl(long? genreId)
    {
        var parameters = new List<string>();

        if (Year.HasValue) parameters.Add($"Year={Year}");
        if (PlatformId.HasValue) parameters.Add($"PlatformId={PlatformId}");
        if (genreId.HasValue) parameters.Add($"GenreId={genreId}");

        return parameters.Any() ? $"/games/best-games/?{string.Join("&", parameters)}" : "/games/best-games/";
    }
}
