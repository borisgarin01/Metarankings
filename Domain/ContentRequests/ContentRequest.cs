namespace Domain.ContentRequests;

/// <summary>
/// Заявка пользователя на добавление игры или фильма, которых нет в базе.
/// </summary>
public sealed record ContentRequest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    /// <summary>
    /// Имя автора заявки (из ApplicationUsers), заполняется при чтении.
    /// </summary>
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("contentType")]
    public ContentRequestType ContentType { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public ContentRequestStatus Status { get; set; }

    /// <summary>
    /// Ответ администратора автору заявки (например, причина отказа).
    /// </summary>
    [JsonPropertyName("adminComment")]
    public string? AdminComment { get; set; }

    [JsonPropertyName("createdTimestamp")]
    public DateTime CreatedTimestamp { get; set; }

    [JsonPropertyName("processedTimestamp")]
    public DateTime? ProcessedTimestamp { get; set; }
}
