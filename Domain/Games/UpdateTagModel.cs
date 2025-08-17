namespace Domain.Games;

public sealed record UpdateTagModel
(
[Required(ErrorMessage = "Title is requeried")]
[MaxLength(255, ErrorMessage = "Title's max length is 255")]
[MinLength(1, ErrorMessage = "Title should be not empty")]
string Title);
