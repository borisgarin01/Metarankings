using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Admin.Games.Developers;

[Authorize(Policy = "Admin")]
public partial class ListDevelopersPage : ComponentBase
{
    public IEnumerable<Developer> Developers { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Developers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Developer>>(@"/api/Games/Developers");
    }
}