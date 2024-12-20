using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels;
public sealed record UpdateGameViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public float? Score { get; set; }

    [Required]
    public string ImageSource { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public long? LocalizationId { get; set; }

    [Required]
    public DateTime? ReleaseDate { get; set; }
}
