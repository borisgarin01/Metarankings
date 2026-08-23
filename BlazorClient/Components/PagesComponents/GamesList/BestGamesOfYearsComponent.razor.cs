namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearsComponent : ComponentBase
{
    [CascadingParameter(Name = "Year")]
    public int? Year { get; set; }

    [CascadingParameter(Name = "PlatformId")]
    public long? PlatformId { get; set; }

    [CascadingParameter(Name = "GenreId")]
    public long? GenreId { get; set; }
}