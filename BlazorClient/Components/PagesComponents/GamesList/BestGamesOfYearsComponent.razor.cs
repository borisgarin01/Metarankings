namespace BlazorClient.Components.PagesComponents.GamesList;

public partial class BestGamesOfYearsComponent : ComponentBase
{
    [CascadingParameter(Name = "Year")]
    public int Year { get; set; }
}
