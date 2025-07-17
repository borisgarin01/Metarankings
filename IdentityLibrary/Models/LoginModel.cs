namespace IdentityLibrary.Models;

public sealed record LoginModel(
    [Required(ErrorMessage = "User email should be set")]
    [DataType(DataType.EmailAddress, ErrorMessage ="Enter a valid user email address")]
    [MinLength(1,ErrorMessage ="User email should be set")]
    string UserEmail,


    [Required(ErrorMessage = "Password should be set")]
    [MaxLength(511,ErrorMessage ="Password is too long")]
    [MinLength(1,ErrorMessage ="Password should be set")]
    [DataType(DataType.Password)]string Password);