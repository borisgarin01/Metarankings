namespace IdentityLibrary.Models;

public sealed record RegisterModel
{
    [Required(ErrorMessage = "Phone number is required")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "User email is required")]
    [DataType(DataType.EmailAddress)]
    public string UserEmail { get; set; }

    [Required(ErrorMessage = "User name is required")]
    [DataType(DataType.Text)]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password confirmation is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and password confirmation are different")]
    public string PasswordConfirmation { get; set; }
}
