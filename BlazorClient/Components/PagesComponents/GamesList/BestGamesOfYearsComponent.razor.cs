namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public int Year { get; set; }
}
