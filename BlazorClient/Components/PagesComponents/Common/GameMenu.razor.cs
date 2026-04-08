namespace BlazorClient.Components.PagesComponents.Common;

public partial class GameMenu : ComponentBase
{
    [Parameter, EditorRequired]
    public int Year { get; set; }
}
