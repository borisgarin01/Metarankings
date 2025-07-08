namespace IdentityLibrary.Models;

public sealed record LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
