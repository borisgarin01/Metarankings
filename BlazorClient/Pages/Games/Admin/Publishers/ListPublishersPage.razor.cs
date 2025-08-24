using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Games.Admin.Publishers;

[Authorize(Policy = "Admin")]
public partial class ListPublishersPage : ComponentBase
{
    public IEnumerable<Publisher> Publishers { get; private set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Publishers = await HttpClient.GetFromJsonAsync<IEnumerable<Publisher>>(@"/api/Publishers");
    }
}
