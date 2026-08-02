namespace Domain.Auth;

public sealed record RefreshToken(
    [property:JsonPropertyName("id")]
    long Id,
    [property:JsonPropertyName("userId")]
    long UserId,
    [property:JsonPropertyName("value")]
    string Value,
    [property:JsonPropertyName("isRevoked")]
    bool IsRevoked,
    [property:JsonPropertyName("createdAt")]
    DateTime CreatedAt);
