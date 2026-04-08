using System.Text.Json.Serialization;

namespace IdentityLibrary.Models;

public sealed record LoginModel
{
    [Required(ErrorMessage = "User email is required")]
    [DataType(DataType.EmailAddress)]
    [JsonPropertyName("userEmail")]
    public string UserEmail { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
