namespace Domain.RequestsModels.Games.Publishers;

public sealed record AddPublisherModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
