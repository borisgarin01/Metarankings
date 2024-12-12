using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Domain;

[Index(nameof(Href), IsUnique = true)]
[Index(nameof(Title), IsUnique = true)]
[Index(nameof(ImageSrc), IsUnique = true)]
public sealed record CollectionItem
{
    public long Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Href { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string ImageSrc { get; set; }
    public long CollectionId { get; set; }
    public Collection Collection { get; set; }
}
