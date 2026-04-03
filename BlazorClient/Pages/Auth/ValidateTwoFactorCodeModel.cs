namespace BlazorClient.Pages.Auth;

public class ValidateTwoFactorCodeModel
{
    [Required]
    [MaxLength(6, ErrorMessage = "Two-factor code must be 6 characters long.")]
    [MinLength(6, ErrorMessage = "Two-factor code must be 6 characters long.")]
    public string TwoFactorCode { get; set; } = string.Empty;
}