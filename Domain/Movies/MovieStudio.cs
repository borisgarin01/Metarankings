namespace Domain.Movies;
public sealed record MovieStudio
(
    [property: Key]
    [property:DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(511, ErrorMessage = "Name max length is 511")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name
);
