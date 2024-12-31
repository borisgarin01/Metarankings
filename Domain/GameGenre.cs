using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public record GameGenre
{
    public long Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Url { get; set; }
    public virtual IEnumerable<Game>? Games { get; set; }
}
