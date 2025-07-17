namespace API.Models.RequestsModels.Games.Genres;

public sealed record AddGenreModel
([property: JsonPropertyName("name")]
string Name
);
