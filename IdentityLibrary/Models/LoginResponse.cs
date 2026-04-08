namespace IdentityLibrary.Models;

public sealed record LoginResponse
{
    [Required]
    public bool RequiresTwoFactor { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public string Message { get; set; }
}
