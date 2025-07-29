namespace IdentityLibrary.Models;

public sealed record LoginModel
{
    [Required(ErrorMessage = "User email is required")]
    [DataType(DataType.EmailAddress)]
    public string UserEmail { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
