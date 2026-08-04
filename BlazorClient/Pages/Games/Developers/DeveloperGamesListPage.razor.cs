using Domain.Games;

namespace BlazorClient.Pages.Games.Developers;

public partial class DeveloperGamesListPage : ComponentBase
{
    [Parameter]
    public int DeveloperId { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public Developer Developer { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Developer = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Developer>($"/api/Games/Developers/{DeveloperId}");
    }
}
