using Domain.Movies.Collections;

namespace BlazorClient.Pages.Movies.Collections;

public partial class Details : CancellableComponentBase
{
    private MoviesCollection moviesCollection;

    [Parameter, EditorRequired]
    public long MovieCollectionId { get; set; }
    public MoviesCollection MoviesCollection
    {
        get => moviesCollection;
        set
        {
            moviesCollection = value;
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
        if (_loadedId == MovieCollectionId)
            return;

        _loadedId = MovieCollectionId;
        IsNotFound = false;
        MoviesCollection = null;

        long requestedId = MovieCollectionId;
        MoviesCollection? loaded;

        try
        {
            loaded = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<MoviesCollection>($"/api/movies/collections/{requestedId}", DisposalToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            loaded = null;
        }

        // Пока шёл запрос, могли перейти к другой подборке — устаревший ответ отбрасываем
        if (requestedId != _loadedId)
            return;

        MoviesCollection = loaded;
        IsNotFound = loaded is null;
    }
}
