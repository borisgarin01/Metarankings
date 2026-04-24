using Domain.Common;
using ViewModels;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class SoonAtCinemasItemComponent : ComponentBase
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
    public string OriginalName { get; set; }

    [Parameter, EditorRequired]
    public DateTime ReleaseDate { get; set; }

    [Parameter, EditorRequired]
    public Link[] Genres { get; set; } = new Link[]
    {
        new Link("https://metarankings.ru/meta/movies/boeviki/", "Боевики")
    };
}
