using Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorClient.Components.PagesComponents;

public partial class GenresComponent
{
    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; }
}
