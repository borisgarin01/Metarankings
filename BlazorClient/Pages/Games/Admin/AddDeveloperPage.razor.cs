using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Games.Admin;

[Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
public partial class AddDeveloperPage : ComponentBase
{
}
