
using Domain.Games.Collections;

namespace BlazorClient.Pages.Collections;

public partial class CollectionsList : ComponentBase
{
    private IEnumerable<GameCollection> gamesCollections;

    [Inject]
    public HttpClient HttpClient { get; set; }

    public IEnumerable<GameCollection> GamesCollections
    {
        get => gamesCollections;
        set
        {
            gamesCollections = value;
            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        GamesCollections = await HttpClient.GetFromJsonAsync<IEnumerable<GameCollection>>(@"/api/Games/Collections");
    }
}
