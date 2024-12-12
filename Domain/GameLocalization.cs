using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Title), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public sealed record GameLocalization
{
    public long Id { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Url { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Title { get; set; }
    public IEnumerable<Game> Games { get; set; }
}
