using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels;
public sealed record AddGameViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public float? Score { get; set; }

    [Required]
    public string ImageSource { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public long? LocalizationId { get; set; }

    [Required]
    public DateTime? ReleaseDate { get; set; }

    public List<GameGenre> GameGenres { get; set; } = new();
    public List<GameDeveloper> Developers { get; set; } = new();
    public List<GamePublisher> Publishers { get; set; } = new();
    public List<GamePlatform> Platforms { get; set; } = new();
}
