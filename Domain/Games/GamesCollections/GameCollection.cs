namespace Domain.Games.Collections;

public sealed record GameCollection
{
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Name { get; set; }

    public List<Game> Games { get; set; } = new();
}
