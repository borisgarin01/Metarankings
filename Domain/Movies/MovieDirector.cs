namespace Domain.Movies;
public sealed record MovieDirector(
    [property: Key]
    [property:DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name
);