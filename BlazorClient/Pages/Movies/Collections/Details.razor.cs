using Domain.Movies.Collections;

namespace BlazorClient.Pages.Movies.Collections;

public partial class Details : ComponentBase
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

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesCollection = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<MoviesCollection>($"/api/movies/collections/{MovieCollectionId}");
    }
}
