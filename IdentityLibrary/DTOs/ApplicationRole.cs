using System.ComponentModel.DataAnnotations;

namespace IdentityLibrary.DTOs;

public class ApplicationRole
{
    public long Id { get; set; }

    [MaxLength(255, ErrorMessage = "Name max length is 255 symbols")]
    [MinLength(1, ErrorMessage = "Name min length is 1 symbol")]
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }


    [MaxLength(255, ErrorMessage = "Normalized name max length is 255 symbols")]
    [MinLength(1, ErrorMessage = "Normalized name min length is 1 symbol")]
    [Required(ErrorMessage = "Normalized name is required")]
    public string NormalizedName { get; set; }
}
