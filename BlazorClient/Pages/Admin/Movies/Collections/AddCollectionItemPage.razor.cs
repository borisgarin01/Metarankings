using Blazored.Toast.Services;
using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Domain.RequestsModels.Movies.Movies;
using WebManagers;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Admin.Movies.Collections;

public partial class AddCollectionItemPage : ComponentBase
{
    public IEnumerable<Movie> MoviesToSelectFrom { get; set; }

    public long SelectedMovieId { get; private set; }

    [Parameter, EditorRequired]
    public long Id { get; set; }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    [Inject]
    public IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel> MoviesCollectionsItemsWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesToSelectFrom = await MoviesWebManager.GetAllAsync();
    }

    public Task SelectMovie(ChangeEventArgs e)
    {
        SelectedMovieId = long.Parse((string)e.Value);

        return Task.CompletedTask;
    }

    public async Task AddMovieCollectionItemAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesCollectionsItemsWebManager.AddAsync(new AddMoviesCollectionItemModel(SelectedMovieId, Id));
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/admin/movies/collections/{Id}/manage-collection");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
