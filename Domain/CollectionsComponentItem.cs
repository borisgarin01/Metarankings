using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed record CollectionsComponentItem
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

    public string ImageSrc { get; set; }

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

