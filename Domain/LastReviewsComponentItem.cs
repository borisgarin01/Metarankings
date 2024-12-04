using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain;
public sealed record LastReviewsComponentItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string LinkHref { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string LinkTitle { get; set; }
}
