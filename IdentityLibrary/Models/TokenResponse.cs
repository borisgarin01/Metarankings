namespace IdentityLibrary.Models;

public sealed record TokenResponse
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Error { get; set; }
}