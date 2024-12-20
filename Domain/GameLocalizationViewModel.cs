using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed class GameLocalizationViewModel
{
    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Url { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Title { get; set; }
}
