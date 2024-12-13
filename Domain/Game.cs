using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Description), IsUnique = true)]
public sealed record Game
{
    public long Id { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    [Required]
    public string Name { get; set; }

    [Range(0, 10)]
    [Required]
    public float? Score { get; set; }

    [MaxLength(255)]
    [MinLength(1)]
    public string ImageSource { get; set; }
    public string Description { get; set; }
    public GameLocalization Localization { get; set; }
    public long LocalizationId { get; set; }
    public IEnumerable<GameDeveloper> Developers { get; set; }
    public IEnumerable<GamePublisher> Publishers { get; set; }
    public IEnumerable<GamePlatform> Platforms { get; set; }
    public IEnumerable<GameGenre> Genres { get; set; }
    public DateTime ReleaseDate { get; set; }
    public IEnumerable<GameTag> Tags { get; set; }
    public IEnumerable<GameCriticReview> CriticsReviews { get; set; }
    public IEnumerable<GameGamerReview> UsersReviews { get; set; }
}
