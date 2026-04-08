using Domain.Games;

namespace BlazorClient.Pages.Games.Developers;

public partial class DeveloperGamesListPage : ComponentBase
{
    [Parameter]
    public int DeveloperId { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public Developer Developer { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Developer = await HttpClient.GetFromJsonAsync<Developer>($"/api/Games/Developers/{DeveloperId}");
    }
}
