namespace API.Controllers.Auth;

public sealed record ConfirmLoginModel(
    [property: JsonPropertyName("userId")] string UserId,
    [property: JsonPropertyName("twoFactorToken")] string TwoFactorToken
    );