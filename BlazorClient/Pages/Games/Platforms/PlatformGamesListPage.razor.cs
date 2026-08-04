using Domain.Games;

namespace BlazorClient.Pages.Games.Platforms;

public partial class PlatformGamesListPage : ComponentBase
{
    [Parameter]
    public int PlatformId { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public Platform Platform { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Platform = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Platform>($"/api/Games/Platforms/{PlatformId}");
    }
}
