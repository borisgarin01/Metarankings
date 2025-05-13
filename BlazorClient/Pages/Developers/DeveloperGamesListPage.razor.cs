using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Developers;

public partial class DeveloperGamesListPage : ComponentBase
{
    [Parameter]
    public int DeveloperId { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public Developer Developer { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Developer = await HttpClient.GetFromJsonAsync<Developer>($"/api/Developers/{DeveloperId}");
    }
}
