using System.ComponentModel.DataAnnotations;

namespace Domain.Movies;
public sealed record MovieDirector
{
    [Key]
    public long Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    public string Name { get; set; }

}
