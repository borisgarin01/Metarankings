using System.Text.Json.Serialization;

namespace IdentityLibrary.Models;

public sealed record AuthResponseDto(
    [property: JsonPropertyName("isAuthSuccessful")] bool IsAuthSuccessful,
    [property: JsonPropertyName("requiredTwoFactor")] bool RequiredTwoFactor,
    [property: JsonPropertyName("errorMessage")] string ErrorMessage,
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken);