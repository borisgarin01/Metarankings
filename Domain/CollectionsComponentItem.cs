using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed record CollectionsComponentItem
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
    public string ImageSrc { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string ImageAlt { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string CategoryTitle { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string CategoryHref { get; set; }
}

