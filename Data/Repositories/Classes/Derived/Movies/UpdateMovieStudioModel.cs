using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Repositories.Classes.Derived.Movies
{
    public sealed record UpdateMovieStudioModel(
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Max length is 255")]
    [MinLength(1, ErrorMessage = "Name should be not empty")]
    [property:JsonPropertyName("name")]
    string Name);
}