using Domain.Games;

namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearByPlatformsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Platform> Platforms { get; set; }

    [CascadingParameter(Name = "PlatformId")]
    public int? PlatformId { get; set; }

    [CascadingParameter(Name = "Year")]
    public int Year { get; set; }
}