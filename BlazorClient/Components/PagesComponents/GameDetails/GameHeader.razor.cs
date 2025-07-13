namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class GameHeader : ComponentBase
{
    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter, EditorRequired]
    public string Description { get; set; }

    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Parameter, EditorRequired]
    public int? ReleaseYear { get; set; }
}
