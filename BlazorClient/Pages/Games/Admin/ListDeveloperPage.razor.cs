using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Games.Admin;

[Authorize(Policy = "Admin")]
public partial class ListDeveloperPage : ComponentBase
{
    public IEnumerable<Developer> Developers { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Developers = await HttpClient.GetFromJsonAsync<IEnumerable<Developer>>(@"/api/Developers");
    }
}
