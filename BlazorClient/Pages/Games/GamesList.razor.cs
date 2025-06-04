using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Games;

public partial class GamesList : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<GameModel> Games { get; set; } = Enumerable.Empty<GameModel>();

    [Parameter]
    public int PageSize { get; set; } = 5; // Default value

    [Parameter]
    public int PageNumber { get; set; } = 1; // Default value

    protected override async Task OnParametersSetAsync()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        if (PageSize < 1)
            PageSize = 5;
        // Fetch data based on the current PageSize and PageNumber
        Games = await HttpClient.GetFromJsonAsync<IEnumerable<GameModel>>($"/api/Games/{PageNumber}/{PageSize}");
    }
}
