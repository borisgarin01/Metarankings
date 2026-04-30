namespace Domain.Movies.MoviesCollections;

public sealed record MoviesCollection
{
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(1)]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public string ImageSource { get; set; }

    public List<Movie> Movies { get; set; } = new();
}
