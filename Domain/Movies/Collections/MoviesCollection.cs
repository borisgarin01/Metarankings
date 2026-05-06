namespace Domain.Movies.Collections;

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

    public List<MoviesCollectionItem> MoviesCollectionItems { get; set; } = new();
}
