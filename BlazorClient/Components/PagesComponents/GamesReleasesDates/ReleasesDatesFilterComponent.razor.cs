using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesReleasesDates;

public partial class ReleasesDatesFilterComponent : ComponentBase
{
    [CascadingParameter(Name = "PlatformId")]
    public long? PlatformId { get; set; }

    [CascadingParameter(Name = "GenreId")]
    public long? GenreId { get; set; }

    [CascadingParameter(Name = "PageNumber")]
    public int? PageNumber { get; set; }

    [CascadingParameter(Name = "PageSize")]
    public long? PageSize { get; set; }

    [CascadingParameter(Name = "Platforms")]
    public IEnumerable<Platform> Platforms { get; set; }

    [CascadingParameter(Name = "Genres")]
    public IEnumerable<Genre> Genres { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private void OnPlatformChanged(ChangeEventArgs e)
    {
        long? platformId = string.IsNullOrEmpty(e.Value?.ToString())
            ? null
            : long.Parse(e.Value.ToString());

        NavigateWithFilters(platformId, GenreId, PageNumber, PageSize);
    }

    private void OnGenreChanged(ChangeEventArgs e)
    {
        long? genreId = string.IsNullOrEmpty(e.Value?.ToString())
            ? null
            : long.Parse(e.Value.ToString());

        NavigateWithFilters(PlatformId, genreId, PageNumber, PageSize);
    }

    private void ResetFilters()
    {
        NavigationManager.NavigateTo("/games/releases-dates");
    }

    private void NavigateWithFilters(long? platformId, long? genreId, long? pageNumber = 1, long? pageSize = 10)
    {
        var parameters = new List<string>();

        if (platformId.HasValue)
            parameters.Add($"PlatformId={platformId}");

        if (genreId.HasValue)
            parameters.Add($"GenreId={genreId}");

        if (PageNumber.HasValue)
            parameters.Add($"PageNumber={pageNumber}");

        if (PageSize.HasValue)
            parameters.Add($"PageSize={pageSize}");

        string url = parameters.Any()
            ? $"/games/releases-dates?{string.Join("&", parameters)}"
            : "/games/releases-dates";

        NavigationManager.NavigateTo(url);
    }
}
