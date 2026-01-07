namespace Domain.Auth;

public sealed record SetTwoFactorEnabledModel(
[property:JsonPropertyName("twoFactorEnabledModel")]
[Required]
bool TwoFactorEnabled);
