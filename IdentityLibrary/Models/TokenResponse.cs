using System.Text.Json.Serialization;

namespace IdentityLibrary.Models;

public sealed record TokenResponse(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("error")] string Error
);