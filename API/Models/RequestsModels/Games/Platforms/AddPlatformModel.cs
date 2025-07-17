namespace API.Models.RequestsModels.Games.Platforms;

public sealed record AddPlatformModel
([property: JsonPropertyName("name")]
string Name
);
