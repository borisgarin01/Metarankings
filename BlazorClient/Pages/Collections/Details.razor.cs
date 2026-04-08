
using Domain.Games.Collections;

namespace BlazorClient.Pages.Collections;

public partial class Details : ComponentBase
{
    private GameCollection gameCollection;

    [Parameter, EditorRequired]
    public long GameCollectionId { get; set; }
    public GameCollection GameCollection
    {
        get => gameCollection;
        set
        {
            gameCollection = value;
            StateHasChanged();
        }
    }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        GameCollection = await HttpClient.GetFromJsonAsync<GameCollection>($"/api/games/collections/{GameCollectionId}");
    }
}
