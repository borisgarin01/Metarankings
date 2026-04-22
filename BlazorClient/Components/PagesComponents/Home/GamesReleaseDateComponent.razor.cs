namespace BlazorClient.Components.PagesComponents.Home;

public partial class GamesReleaseDateComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<GamesReleaseDateItemComponent> GamesReleaseDateItemComponents { get; set; }
}
