namespace API.Models.RequestsModels.Games.Developers;

public sealed record UpdateDeveloperModel
([property: JsonPropertyName("name")]
string Name
);
