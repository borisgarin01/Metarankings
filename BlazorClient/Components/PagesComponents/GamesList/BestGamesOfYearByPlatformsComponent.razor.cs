namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearByPlatformsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public int Year { get; set; }
}
