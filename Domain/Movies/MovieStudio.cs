namespace Domain.Movies;
public sealed record MovieStudio
(
    [property: Key]
    [property:DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    [property:Required(ErrorMessage = "Name is required")]
    [property:MaxLength(511, ErrorMessage = "Name max length is 511")]
    [property:MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name
);
