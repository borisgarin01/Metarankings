using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages;

public partial class Home : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<Game> Games { get; set; }

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
        Games = await HttpClient.GetFromJsonAsync<IEnumerable<Game>>($"/api/Games/{PageNumber}/{PageSize}");
    }
}
