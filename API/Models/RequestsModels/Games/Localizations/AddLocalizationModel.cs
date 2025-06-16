using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Games.Localizations;

public sealed class AddLocalizationModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
