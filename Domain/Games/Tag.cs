namespace Domain.Games;

public sealed record Tag(
    [property: Key]
    [property: DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    [property:Required(ErrorMessage = "Title is requeried")]
    [property:MaxLength(255, ErrorMessage = "Title's max length is 255")]
    [property:MinLength(1, ErrorMessage = "Title should be not empty")]
    string Title);
