namespace Domain.Auth;

public sealed record AuthenticationScheme(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("displayName")] string? DisplayName);