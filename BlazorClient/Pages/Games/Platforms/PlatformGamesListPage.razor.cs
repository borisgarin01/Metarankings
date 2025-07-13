using Domain.Games;

namespace BlazorClient.Pages.Games.Platforms;

public partial class PlatformGamesListPage : ComponentBase
{
    [Parameter]
    public int PlatformId { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public Platform Platform { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Platform = await HttpClient.GetFromJsonAsync<Platform>($"/api/Platforms/{PlatformId}");
    }
}
