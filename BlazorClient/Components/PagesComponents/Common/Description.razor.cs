namespace BlazorClient.Components.PagesComponents.Common;

public partial class Description : ComponentBase
{
    [Parameter, EditorRequired]
    public string TextContent { get; set; }
}
