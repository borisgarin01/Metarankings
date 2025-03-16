using BlazorClient.Components.PagesComponents;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages;

public partial class Home : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<Game> Games { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Games = await HttpClient.GetFromJsonAsync<IEnumerable<Game>>("/api/Games/1/5");
    }
}
