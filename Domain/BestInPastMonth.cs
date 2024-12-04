using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public sealed record BestInPastMonth
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Href { get; set; }

    [Range(0,10)]
    public float Score { get; set; }

    public string ScoreStyle { get { return $"small-score mark-{Math.Round(Score)}"; } }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string ImageSrc { get; set; }
}
