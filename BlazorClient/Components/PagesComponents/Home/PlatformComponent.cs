namespace BlazorClient.Components.PagesComponents.Home;

public partial class PlatformComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public Dictionary<string, string> Platforms { get; set; }
}