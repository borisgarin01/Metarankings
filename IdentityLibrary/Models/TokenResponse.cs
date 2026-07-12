using System.Text.Json.Serialization;

namespace IdentityLibrary.Models;

public sealed record TokenResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("tokenExpired")] long TokenExpired,
    [property: JsonPropertyName("refreshToken")] string RefreshToken
);