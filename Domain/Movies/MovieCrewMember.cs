namespace Domain.Movies;

/// <summary>
/// Участник съёмочной группы конкретного фильма: персона и её роль.
/// </summary>
public sealed record MovieCrewMember
{
    /// <summary>
    /// Идентификатор персоны (MoviesPersons.Id).
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public MovieCrewRole Role { get; set; }
}
