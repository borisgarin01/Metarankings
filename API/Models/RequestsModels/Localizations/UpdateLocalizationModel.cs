using System.Text.Json.Serialization;

namespace API.Models.RequestsModels.Localizations;

public class UpdateLocalizationModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }
}
