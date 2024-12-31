using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
public class Game
{
    public long Id { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Name { get; set; }

    [Range(0, 10)]
    [Required]
    public float? Score { get; set; }

    [MaxLength(511)]
    [MinLength(1)]
    public string ImageSource { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string Description { get; set; }

    public GameLocalization Localization { get; set; }
    public long LocalizationId { get; set; }

    public virtual ICollection<GameDeveloper> Developers { get; set; } = new List<GameDeveloper>();
    public virtual ICollection<GamePublisher> Publishers { get; set; } = new List<GamePublisher>();
    public virtual ICollection<GamePlatform> Platforms { get; set; } = new List<GamePlatform>();
    public virtual ICollection<GameGenre> Genres { get; set; } = new List<GameGenre>();

    public DateTime ReleaseDate { get; set; }

    public virtual ICollection<GameTag> Tags { get; set; } = new List<GameTag>();
    public virtual ICollection<GameCriticReview> CriticsReviews { get; set; } = new List<GameCriticReview>();
    public virtual ICollection<GameGamerReview> UsersReviews { get; set; } = new List<GameGamerReview>();
    public virtual ICollection<Trailer> Trailers { get; set; } = new List<Trailer>();
}