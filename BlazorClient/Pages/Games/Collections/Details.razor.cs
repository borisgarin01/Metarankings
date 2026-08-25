using Domain.Games.Collections;

namespace BlazorClient.Pages.Games.Collections;

public partial class Details : ComponentBase
{
    private GamesCollection gameCollection;

    [Parameter, EditorRequired]
    public long GameCollectionId { get; set; }

    public GamesCollection GameCollection
    {
        get => gameCollection;
        set
        {
            gameCollection = value;
            StateHasChanged();
        }
    }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        GameCollection = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<GamesCollection>($"/api/games/collections/{GameCollectionId}");
    }
}
