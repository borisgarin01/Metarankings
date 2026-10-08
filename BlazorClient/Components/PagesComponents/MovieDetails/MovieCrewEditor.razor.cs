using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;

namespace BlazorClient.Components.PagesComponents.MovieDetails;

/// <summary>
/// Ввод съёмочной группы фильма: по текстовому полю на каждую роль.
/// </summary>
public partial class MovieCrewEditor : ComponentBase
{
    private static readonly char[] NamesSeparators = { ',', ';', '\n', '\r' };

    /// <summary>
    /// Текст поля для каждой роли; словарь изменяется компонентом на месте.
    /// </summary>
    [Parameter, EditorRequired]
    public Dictionary<MovieCrewRole, string> Texts { get; set; } = default!;

    public static Dictionary<MovieCrewRole, string> CreateTexts(IEnumerable<MovieCrewMember>? crew = null)
    {
        return Enum.GetValues<MovieCrewRole>().ToDictionary(
            role => role,
            role => string.Join(", ", (crew ?? Enumerable.Empty<MovieCrewMember>()).Where(c => c.Role == role).Select(c => c.Name)));
    }

    public static List<MovieCrewMemberModel> ToModels(Dictionary<MovieCrewRole, string> texts)
    {
        return texts
            .SelectMany(roleText => (roleText.Value ?? string.Empty)
                .Split(NamesSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(name => new MovieCrewMemberModel(name, roleText.Key)))
            .ToList();
    }
}
