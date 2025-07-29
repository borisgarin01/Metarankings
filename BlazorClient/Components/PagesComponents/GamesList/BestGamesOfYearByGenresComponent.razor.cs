namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearByGenresComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public short Year { get; set; }
}
