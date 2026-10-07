namespace Domain.Reviews;

/// <summary>
/// Рецензия критика (издания) на игру или фильм. Добавляется администратором.
/// </summary>
public sealed record CriticReview
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Id игры или фильма.
    /// </summary>
    [JsonPropertyName("entityId")]
    public long EntityId { get; set; }

    [JsonPropertyName("publication")]
    public string Publication { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("score")]
    public float Score { get; set; }

    [JsonPropertyName("textContent")]
    public string? TextContent { get; set; }

    /// <summary>
    /// Ссылка на оригинал рецензии.
    /// </summary>
    [JsonPropertyName("sourceUrl")]
    public string? SourceUrl { get; set; }

    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
}
