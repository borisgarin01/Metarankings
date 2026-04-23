namespace BlazorClient.Components.PagesComponents.Home;

public partial class CollectionsComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<CollectionsItemComponent> CollectionsItemComponents { get; set; }
}
