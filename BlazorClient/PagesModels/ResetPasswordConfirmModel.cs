using System.ComponentModel.DataAnnotations;

namespace BlazorClient.PagesModels
{
    public sealed class ResetPasswordConfirmModel : ComponentBase
    {
        [Required]
        public string ResetPasswordToken { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Password and password confirmation don't match")]
        public string NewPasswordConfirmation { get; set; }
    }
}
