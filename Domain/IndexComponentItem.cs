using System.ComponentModel.DataAnnotations;

namespace Domain;

public sealed record IndexComponentItem
{
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string ItemHref { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Title { get; set; }

    [Range(0, 10)]
    public float Score { get; set; }

    [MinLength(1)]
    public string Description { get; set; }
    public DateOnly ReleaseDate { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    public string ImageSrc { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    public string ImageAlt { get; set; }
}
