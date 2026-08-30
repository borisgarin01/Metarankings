namespace Domain.Movies;

public sealed record Genre
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("movies")]
    public List<Movie> Movies { get; set; } = new();
}
