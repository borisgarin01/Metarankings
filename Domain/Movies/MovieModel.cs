using System.Text.Json.Serialization;

namespace Domain.Movies;

public sealed record MovieModel
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("originalName")]
    public required string OriginalName { get; set; }

    [JsonPropertyName("imageSource")]
    public required string ImageSource { get; set; }

    [JsonPropertyName("premierDate")]
    public DateTime? PremierDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("movieGenres")]
    public List<MovieGenre> MovieGenres { get; set; } = new();

    [JsonPropertyName("moviesStudios")]
    public List<MovieStudio> MoviesStudios { get; set; } = new();

    [JsonPropertyName("moviesDirectors")]
    public List<MovieDirector> MoviesDirectors { get; set; } = new();
}
