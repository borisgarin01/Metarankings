using Domain.Games;
using System.Net;

namespace BlazorClient.Pages.Games.Admin;

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
