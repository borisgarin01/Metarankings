namespace BlazorClient.PagesModels;

public sealed class SetTwoFactorEnabledModel : ComponentBase
{
    [Required]
    public bool TwoFactorEnabled { get; set; }
}
