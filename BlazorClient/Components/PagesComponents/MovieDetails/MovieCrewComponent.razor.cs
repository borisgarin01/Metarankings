using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

public partial class MovieCrewComponent : ComponentBase
{
    [Parameter]
    public IEnumerable<MovieDirector> Directors { get; set; } = Enumerable.Empty<MovieDirector>();

    [Parameter]
    public IEnumerable<MovieCrewMember> Crew { get; set; } = Enumerable.Empty<MovieCrewMember>();

    /// <summary>
    /// Свойство schema.org/Movie для роли; null — у schema.org нет подходящего свойства.
    /// </summary>
    private static string? ItemProp(MovieCrewRole role) => role switch
    {
        MovieCrewRole.Actor => "actor",
        MovieCrewRole.Producer => "producer",
        MovieCrewRole.Composer => "musicBy",
        MovieCrewRole.Screenwriter => "author",
        _ => null
    };
}
