using Domain.Games;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClient.Pages.Admin.Games.Publishers;

[Authorize(Policy = "Admin")]
public partial class ListPublishersPage : ComponentBase
{
    private IEnumerable<Publisher> publishers;

    public IEnumerable<Publisher> Publishers
    {
        get => publishers;
        private set
        {
            publishers = value;
            StateHasChanged();
        }
    }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Publishers = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<IEnumerable<Publisher>>(@"/api/Games/Publishers");
    }
}
