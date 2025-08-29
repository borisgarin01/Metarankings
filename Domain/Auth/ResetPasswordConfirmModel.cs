namespace Domain.Auth;

public sealed record ResetPasswordConfirmModel(
    [property:JsonPropertyName("email")]
    [Required]
    string Email,
    [property:JsonPropertyName("newPassword")]
    [Required]
    string NewPassword,
    [property:JsonPropertyName("resetPasswordToken")]
    [Required]
    string ResetPasswordToken);
