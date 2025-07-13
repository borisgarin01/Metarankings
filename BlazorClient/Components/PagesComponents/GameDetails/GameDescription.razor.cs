namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameDescription : ComponentBase
{
    [Parameter, EditorRequired]
    public string Description { get; set; }
}
