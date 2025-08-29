namespace Domain.Auth;

public sealed record ResetPasswordModel(
    [property:JsonPropertyName("email")]
    [Required]
    string Email);
