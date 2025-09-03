namespace BlazorClient.Components.PagesComponents.Common;

public sealed partial class Thumbnail : ComponentBase
{
    [Parameter, EditorRequired]//Games/Movies
    [AllowedValues("Games", "Movies")]
    public string Prefix { get; set; }

    [Parameter, EditorRequired]
    public int ReleaseYear { get; set; }

    [Parameter, EditorRequired]
    public int ReleaseMonth { get; set; }

    [Parameter, EditorRequired]
    public string ImageSource { get; set; }

    [Parameter, EditorRequired]
    public string Name { get; set; }
}
