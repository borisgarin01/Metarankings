namespace Domain.Movies;
public sealed record MovieStudio
{
    [Key]
    public long Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(511, ErrorMessage = "Name max length is 511")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    public string Name { get; set; }
}
