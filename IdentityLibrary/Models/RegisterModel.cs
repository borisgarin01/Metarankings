namespace IdentityLibrary.Models;

public sealed record RegisterModel
{
    public string PhoneNumber { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}
