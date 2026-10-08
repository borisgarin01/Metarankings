using Domain.Movies;

namespace Domain.RequestsModels.Movies.Movies;

/// <summary>
/// Участник съёмочной группы при добавлении/обновлении фильма. Персона ищется по имени
/// и создаётся, если её ещё нет. Порядок в списке сохраняется внутри роли.
/// </summary>
public sealed record MovieCrewMemberModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("role")] MovieCrewRole Role);
