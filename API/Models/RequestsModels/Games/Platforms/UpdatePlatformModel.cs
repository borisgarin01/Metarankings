namespace API.Models.RequestsModels.Games.Platforms;

public sealed record UpdatePlatformModel
([property: JsonPropertyName("name")]
string Name
);
