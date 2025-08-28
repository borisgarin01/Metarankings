namespace Domain.Common;

public sealed record Link(
    [property:Required]
    [property:MaxLength(255,ErrorMessage ="Title is too long")]
    [property:MinLength(1,ErrorMessage ="Title should be not empty")]
string Title,
    [property:Required]
    [property:MaxLength(1023,ErrorMessage ="Url is too long")]
    [property:MinLength(1,ErrorMessage ="Url should be not empty")]
    string Url);
