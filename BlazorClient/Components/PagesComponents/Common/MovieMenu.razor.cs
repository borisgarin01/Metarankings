namespace BlazorClient.Components.PagesComponents.Common;

public partial class MovieMenu : ComponentBase
{
    [Parameter, EditorRequired]
    public int? Year { get; set; }
}