using Domain.Games.Collections;
using Domain.Movies.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Movies.Collections;
using WebManagers;

namespace BlazorClient.Pages.Movies.Collections;

public partial class CollectionsList : ComponentBase
{
    private IEnumerable<MoviesCollection> moviesCollections;

    [Inject]
    public IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> MoviesCollectionsWebManager { get; set; }

    public IEnumerable<MoviesCollection> MoviesCollections
    {
        get => moviesCollections;
        set
        {
            moviesCollections = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        MoviesCollections = await MoviesCollectionsWebManager.GetAllAsync();
    }

    public async Task DeleteCollectionAsync(long id)
    {
        await MoviesCollectionsWebManager.DeleteAsync(id);
        MoviesCollections = await MoviesCollectionsWebManager.GetAllAsync();
    }
}
