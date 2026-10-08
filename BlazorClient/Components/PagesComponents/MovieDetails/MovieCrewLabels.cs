using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

/// <summary>
/// Подписи ролей съёмочной группы.
/// </summary>
public static class MovieCrewLabels
{
    public static string GetRoleLabel(MovieCrewRole role) => role switch
    {
        MovieCrewRole.Actor => "Актёры",
        MovieCrewRole.Producer => "Продюсер",
        MovieCrewRole.Cinematographer => "Оператор",
        MovieCrewRole.Screenwriter => "Сценарист",
        MovieCrewRole.Composer => "Композитор",
        MovieCrewRole.Artist => "Художник",
        MovieCrewRole.Editor => "Монтажёр",
        _ => role.ToString()
    };
}
