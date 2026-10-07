namespace Domain.RequestsModels.CriticsReviews;

/// <summary>
/// Модель добавления / изменения рецензии критика.
/// </summary>
public sealed class CriticReviewModel : IValidatableObject
{
    /// <summary>
    /// Id игры или фильма. При изменении рецензии не используется.
    /// </summary>
    [JsonPropertyName("entityId")]
    public long EntityId { get; set; }

    [JsonPropertyName("publication")]
    [Required(ErrorMessage = "Укажите издание")]
    [MaxLength(255, ErrorMessage = "Название издания слишком длинное")]
    public string Publication { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    [MaxLength(255, ErrorMessage = "Имя автора слишком длинное")]
    public string? Author { get; set; }

    [JsonPropertyName("score")]
    [Range(0.0f, 10.0f, ErrorMessage = "Оценка должна быть от 0 до 10")]
    public float Score { get; set; }

    [JsonPropertyName("textContent")]
    [MaxLength(10000, ErrorMessage = "Текст слишком длинный")]
    public string? TextContent { get; set; }

    [JsonPropertyName("sourceUrl")]
    [MaxLength(1023, ErrorMessage = "Ссылка слишком длинная")]
    public string? SourceUrl { get; set; }

    [JsonPropertyName("date")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Ссылка выводится в href, поэтому допускаются только http(s)
        if (!string.IsNullOrWhiteSpace(SourceUrl)
            && !(Uri.TryCreate(SourceUrl.Trim(), UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
        {
            yield return new ValidationResult("Ссылка должна начинаться с http:// или https://", new[] { nameof(SourceUrl) });
        }
    }
}
