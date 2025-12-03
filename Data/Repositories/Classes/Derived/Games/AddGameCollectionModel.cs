namespace Data.Repositories.Classes.Derived.Games;

public sealed record AddGameCollectionModel(
    [property:JsonPropertyName("name")]
    [Required(ErrorMessage ="Name is required")]
    string Name);
