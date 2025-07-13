namespace IdentityLibrary.DTOs;

public class ApplicationUser
{
    public long Id { get; set; }

    [MaxLength(255, ErrorMessage = "User name max length is 255")]
    [MinLength(1, ErrorMessage = "User name should be not empty")]
    [Required(ErrorMessage = "User name is required")]
    public string UserName { get; set; }


    [MaxLength(255, ErrorMessage = "Normalized user name max length is 255")]
    [MinLength(1, ErrorMessage = "Normalized user name should be not empty")]
    [Required(ErrorMessage = "Normalized user name is required")]
    public string NormalizedUserName { get; set; }

    [MaxLength(255, ErrorMessage = "Email max length is 255")]
    [MinLength(1, ErrorMessage = "Email should be not empty")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [MaxLength(255, ErrorMessage = "Normalized email max length is 255")]
    [MinLength(1, ErrorMessage = "Normalized email should be not empty")]
    [Required(ErrorMessage = "Normalized email is required")]
    public string NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    [MaxLength(511, ErrorMessage = "Password hash max length is 511")]
    [MinLength(1, ErrorMessage = "Pasword hash should be not empty")]
    [Required(ErrorMessage = "Password hash is required")]
    public string PasswordHash { get; set; }

    [MaxLength(31, ErrorMessage = "Phone number max length is 31")]
    [MinLength(1, ErrorMessage = "Phone number should be not empty")]
    [Required(ErrorMessage = "Phone number is required")]
    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }
}
