namespace BlazorClient.Pages.Games.Games;

public partial class BestGamesOfYearsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public int Year { get; set; }
}
