namespace Domain.Auth;

public sealed record ConfirmLoginModel(
    [property: JsonPropertyName("userId")] string UserId,
    [property: JsonPropertyName("twoFactorToken")] string TwoFactorToken
    );