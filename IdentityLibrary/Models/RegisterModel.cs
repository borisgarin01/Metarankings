namespace IdentityLibrary.Models;

public sealed record RegisterModel(
    [Required(ErrorMessage = "Phone number should be set")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Enter valid phone number")]
    [MaxLength(51, ErrorMessage = "Phone number is too long")]
    [MinLength(1, ErrorMessage = "Phone number should be set")]
    string PhoneNumber,

    [Required(ErrorMessage = "User email should be set")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Enter a valid user email address")]
    [MaxLength(511, ErrorMessage = "User email is too long")]
    [MinLength(1, ErrorMessage = "User email should be set")]
    string UserEmail,

    [Required(ErrorMessage = "User name should be set")]
    [MaxLength(511, ErrorMessage = "User name is too long")]
    [MinLength(1, ErrorMessage = "User name should be set")]
    string UserName,

    [Required(ErrorMessage = "Password should be set")]
    [MaxLength(511, ErrorMessage = "Password is too long")]
    [MinLength(1, ErrorMessage = "Password should be set")]
    [DataType(DataType.Password)]
    string Password,

    [Required(ErrorMessage = "Password confirmation should be set")]
    [MaxLength(511, ErrorMessage = "Password confirmation is too long")]
    [MinLength(1, ErrorMessage = "Password confirmation should be set")]
    [DataType(DataType.Password)]
    string PasswordConfirmation
);