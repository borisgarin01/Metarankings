namespace API.Models.RequestsModels.Games.Developers;

public sealed record AddDeveloperModel
([property: JsonPropertyName("name")]
string Name
);
