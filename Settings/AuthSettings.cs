namespace Settings;

public sealed record AuthSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
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
    public int TokenLifetimeHours { get; set; }
    public Telegram Telegram { get; set; }
}
