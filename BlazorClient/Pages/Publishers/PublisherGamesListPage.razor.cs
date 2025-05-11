using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Publishers;

public partial class PublisherGamesListPage : ComponentBase
{
    [Parameter]
    public int PublisherId { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public Publisher Publisher { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Publisher = await HttpClient.GetFromJsonAsync<Publisher>($"/api/Publishers/{PublisherId}");
    }
}