using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Admin.Games.Publishers;

[Authorize(Policy = "Admin")]
public partial class ListPublishersPage : ComponentBase
{
    public IEnumerable<Publisher> Publishers { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Publishers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Publisher>>(@"/api/Games/Publishers");
    }
}
