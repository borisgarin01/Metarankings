using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed record NewsComponentItem
{
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string LinkHref { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string LinkTitle { get; set; }
    public string ImageSrc { get; set; }
    public string ImageAlt { get; set; }
}
