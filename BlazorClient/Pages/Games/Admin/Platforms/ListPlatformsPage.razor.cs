using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Games.Admin.Platforms;

[Authorize(Policy = "Admin")]
public partial class ListPlatformsPage : ComponentBase
{
    public IEnumerable<Platform> Platforms { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Platforms = await HttpClient.GetFromJsonAsync<IEnumerable<Platform>>(@"/api/Platforms");
    }
}