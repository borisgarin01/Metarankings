using System.ComponentModel.DataAnnotations;

namespace API.Models.Identity;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(255, ErrorMessage = "Email max length is 255")]
    [MinLength(4, ErrorMessage = "Email min length is 4")]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [MaxLength(255, ErrorMessage = "Phone max length is 255")]
    [MinLength(4, ErrorMessage = "Phone min length is 4")]
    [Phone]
    [DataType(DataType.PhoneNumber)]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MaxLength(63, ErrorMessage = "Password max length is 63")]
    [MinLength(1, ErrorMessage = "Password min length is 1")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}