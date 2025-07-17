namespace API.Models.RequestsModels.Games.Publishers;

public sealed record AddPublisherModel
([property: JsonPropertyName("name")]
string Name
);
