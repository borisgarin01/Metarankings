namespace IdentityLibrary.Services;

public sealed record JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
