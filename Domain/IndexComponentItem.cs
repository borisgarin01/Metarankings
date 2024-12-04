using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public sealed record IndexComponentItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string ItemHref { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Title { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public float Score { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Description { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public DateOnly? ReleaseDate { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string ImageSrc { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string ImageAlt { get; set; }
}
