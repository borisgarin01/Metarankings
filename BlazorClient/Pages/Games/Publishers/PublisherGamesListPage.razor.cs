using Domain.Games;

namespace BlazorClient.Pages.Games.Publishers;

public partial class PublisherGamesListPage : ComponentBase
{
    [Parameter]
    public int PublisherId { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public Publisher Publisher { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Publisher = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Publisher>($"/api/Games/Publishers/{PublisherId}");
    }
}