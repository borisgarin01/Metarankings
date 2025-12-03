namespace Data.Repositories.Classes.Derived.Games;

public sealed record AddGameCollectionModel(
    [property:JsonPropertyName("name")]
    [Required(ErrorMessage ="Name is required")]
    [MinLength(1, ErrorMessage ="Name should be set")]
    [MaxLength(255, ErrorMessage ="Name is too long")]
    string Name,
    [property:JsonPropertyName("description")]
    [Required(ErrorMessage ="Description is required")]
    [MinLength(1, ErrorMessage ="Description should be set")]
    string Description);
