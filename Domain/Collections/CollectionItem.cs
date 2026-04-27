namespace Domain.Collections;

public sealed record CollectionItem(
    [property: JsonPropertyName("href")]
    [Required]
    string Href,
    [property:JsonPropertyName("title")]
    [Required]
    string Title,
    [property:JsonPropertyName("imageSrc")]
    [Required]
    string ImageSrc);