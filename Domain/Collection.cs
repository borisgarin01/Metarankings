using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
public record Collection
{
    public long Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Name { get; set; }

    [JsonIgnore]
    public virtual IEnumerable<CollectionItem> CollectionItems { get; set; }
}
