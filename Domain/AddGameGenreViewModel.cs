using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed class AddGameGenreViewModel
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Url { get; set; }
}
