namespace BlazorClient.Components.PagesComponents.Home;

public partial class CollectionsItemComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public string Title { get; set; }

    [Parameter, EditorRequired]
    public string Href { get; set; }

    [Parameter, EditorRequired]
    public string ImageSource { get; set; }

    [Parameter, EditorRequired]
    public string ImageAlt { get; set; }
}
