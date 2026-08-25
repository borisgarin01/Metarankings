using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearByPlatformsComponent : ComponentBase
{
    [CascadingParameter(Name = "Year")]
    public int? Year { get; set; }

    [CascadingParameter(Name = "PlatformId")]
    public long? PlatformId { get; set; }

    [CascadingParameter(Name = "GenreId")]
    public long? GenreId { get; set; }

    [CascadingParameter(Name = "Platforms")]
    public IEnumerable<Platform>? Platforms { get; set; }

    private string BuildUrl(long? platformId = null)
    {
        var parameters = new List<string>();

        if (Year.HasValue) parameters.Add($"Year={Year}");
        if (GenreId.HasValue) parameters.Add($"GenreId={GenreId}");
        if (platformId.HasValue) parameters.Add($"PlatformId={platformId}");

        return parameters.Any() ? $"/games/best-games/?{string.Join("&", parameters)}" : "/games/best-games/";
    }
}