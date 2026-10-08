namespace Domain.Movies;

/// <summary>
/// Роль участника в съёмочной группе фильма. Значения хранятся в MoviesMoviesPersons.Role.
/// Режиссёры хранятся отдельно (MoviesDirectors).
/// </summary>
public enum MovieCrewRole : short
{
    Actor = 1,
    Producer = 2,
    Cinematographer = 3,
    Screenwriter = 4,
    Composer = 5,
    Artist = 6,
    Editor = 7
}
