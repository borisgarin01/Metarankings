namespace Settings;

public sealed record AuthSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string AccessSecret { get; set; }
    public string RefreshSecret { get; set; }
    public string AdminEmail { get; set; }
    public string AdminPassword { get; set; }
    public string Authority { get; set; }
    public bool RequireExpirationTime { get; set; }
    public bool RequireSignedTokens { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public bool ValidateIssuer { get; set; }
    public string ValidIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public string ValidAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public int AccessTokenLifetimeMinutes { get; set; }
    public int RefreshTokenLifetimeDays { get; set; }
    public Telegram Telegram { get; set; }
}
