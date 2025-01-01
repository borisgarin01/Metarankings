using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Url), IsUnique = true)]
public record GamePlatform
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

    [JsonIgnore]
    public virtual IEnumerable<Game>? Games { get; set; }
}
