namespace Domain.Movies;

/// <summary>
/// Участник съёмочной группы (актёр, продюсер, оператор и т.д.). Роль задаётся связью с фильмом.
/// </summary>
public sealed record MoviePerson
(
    [property: Key]
    [property:DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    long Id,

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(511, ErrorMessage = "Name max length is 511")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    string Name
);
