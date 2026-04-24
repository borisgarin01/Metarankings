using Domain.Common;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class GamesReleaseDateItemComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public string Href { get; set; }

    [Parameter, EditorRequired]
    public string Title { get; set; }

    [Parameter, EditorRequired]
    public string ImageSource { get; set; }

    [Parameter, EditorRequired]
    public string ImageAlt { get; set; }

    [Parameter, EditorRequired]
    public string ItemName { get; set; }

    [Parameter, EditorRequired]
    public Link[] Platforms { get; set; }

    [Parameter, EditorRequired]
    public Link[] Genres { get; set; }

    [Parameter, EditorRequired]
    public DateTime ReleaseDate { get; set; }
}
