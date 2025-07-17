namespace API.Models.RequestsModels.Games.Genres;

public sealed record UpdateGenreModel
([property: JsonPropertyName("name")]
string Name
);
