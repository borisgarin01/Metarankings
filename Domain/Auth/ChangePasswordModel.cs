namespace Domain.Auth;

public sealed record ChangePasswordModel(
    [property: JsonPropertyName("currentPassword")] string? CurrentPassword,
    [property: JsonPropertyName("newPassword")] string NewPassword);