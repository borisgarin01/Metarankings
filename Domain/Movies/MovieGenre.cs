namespace Domain.Movies;
public sealed record MovieGenre(
    [property:Key]
    [property:DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    [property:Required(ErrorMessage = "Name is required")]
    [property:MaxLength(255, ErrorMessage = "Name max length is 255")]
    [property:MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name
);
