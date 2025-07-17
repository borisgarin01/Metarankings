namespace IdentityLibrary.Models;

public sealed record LoginModel(
    [property: Required(ErrorMessage = "User email should be set")]
    [property:DataType(DataType.EmailAddress, ErrorMessage ="Enter a valid user email address")]
    [property:MaxLength(511,ErrorMessage ="User email is too long")]
    [property:MinLength(1,ErrorMessage ="User email should be set")]
    string UserEmail,


    [property: Required(ErrorMessage = "Password should be set")]
    [property: MaxLength(511,ErrorMessage ="Password is too long")]
    [property:MinLength(1,ErrorMessage ="Password should be set")]
    [property:DataType(DataType.Password)]string Password);