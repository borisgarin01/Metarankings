using Domain.Games.Collections;

namespace BlazorClient.Pages.Games.Collections;

public partial class Details : CancellableComponentBase
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

    public bool IsNotFound { get; private set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    private long _loadedId = -1;

    protected override async Task OnParametersSetAsync()
    {
        // Компонент переиспользуется при переходе между подборками — грузим только при смене Id
        if (_loadedId == GameCollectionId)
            return;

        _loadedId = GameCollectionId;
        IsNotFound = false;
        GameCollection = null;

        long requestedId = GameCollectionId;
        GamesCollection? loaded;

        try
        {
            loaded = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<GamesCollection>($"/api/games/collections/{requestedId}", DisposalToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            loaded = null;
        }

        // Пока шёл запрос, могли перейти к другой подборке — устаревший ответ отбрасываем
        if (requestedId != _loadedId)
            return;

        GameCollection = loaded;
        IsNotFound = loaded is null;
    }
}
