using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.MoviesList;

public partial class MoviePost : ComponentBase
{
    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Parameter, EditorRequired]
    public required string Name { get; set; }

    [Parameter, EditorRequired]
    public required string Image { get; set; }

    [Parameter, EditorRequired]
    public int? ViewersScoresCount { get; set; }

    [Parameter, EditorRequired]
    public int? CriticsScoresCount { get; set; }

    [Parameter, EditorRequired]
    public required IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();

    [Parameter, EditorRequired]
    public DateOnly? ReleaseDate { get; set; }

    [Parameter, EditorRequired]
    public required string Description { get; set; }
}
