using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Title), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public sealed record GameTag
{
    public long Id { get; set; }
    public IEnumerable<Game> Games { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Url { get; set; }
}
