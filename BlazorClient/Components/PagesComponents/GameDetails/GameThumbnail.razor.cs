namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameThumbnail : ComponentBase
{

    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter, EditorRequired]
    public int ReleaseYear { get; set; }

    [Parameter, EditorRequired]
    public int ReleaseMonth { get; set; }

    [Parameter, EditorRequired]
    public string ImageSource { get; set; }
}
