namespace API.Models.RequestsModels.Games.Publishers;

public sealed record UpdatePublisherModel
([property: JsonPropertyName("name")]
string Name
);
