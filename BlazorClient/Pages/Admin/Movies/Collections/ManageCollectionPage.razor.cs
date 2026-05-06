using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.Collections;

public partial class ManageCollectionPage : ComponentBase
{
    [Inject]
    public IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> MoviesCollectionWebManager { get; set; }

    [Inject]
    public IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> MoviesCollectionsItemsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    private MoviesCollection moviesCollection;

    [Parameter, EditorRequired]
    public long Id { get; set; }

    public MoviesCollection MoviesCollection
    {
        get => moviesCollection;
        private set
        {
            moviesCollection = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        MoviesCollection = await MoviesCollectionWebManager.GetAsync(Id);
    }

    public async Task DeleteGameCollectionAsync(long id)
    {
        await MoviesCollectionWebManager.DeleteAsync(id);
        NavigationManager.NavigateTo("/admin");
    }

    public async Task DeleteMovieFromCollectionAsync(long id)
    {
        await MoviesCollectionsItemsWebManager.DeleteAsync(id);
        MoviesCollection = await MoviesCollectionWebManager.GetAsync(MoviesCollection.Id);
    }
}
