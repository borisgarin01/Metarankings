namespace API.Models.RequestsModels.Games.Localizations;

public class UpdateLocalizationModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
