using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain;

public sealed record GamePlatform
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Href { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Name { get; set; }
}