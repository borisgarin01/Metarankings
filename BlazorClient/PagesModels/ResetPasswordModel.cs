using System.ComponentModel.DataAnnotations;

namespace BlazorClient.PagesModels
{
    public sealed class ResetPasswordModel : ComponentBase
    {
        [Required]
        public string Email { get; set; }
    }
}
