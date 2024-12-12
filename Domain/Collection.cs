using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
public sealed record Collection
{
    public long Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Name { get; set; }
    public IEnumerable<CollectionItem> CollectionItems { get; set; }
}
