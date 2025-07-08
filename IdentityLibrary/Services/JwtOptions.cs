namespace IdentityLibrary.Services;

public sealed record JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiresHours { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
