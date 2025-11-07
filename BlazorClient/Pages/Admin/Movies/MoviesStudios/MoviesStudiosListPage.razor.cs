using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesStudios;

public partial class MoviesStudiosListPage : ComponentBase
{
    public IEnumerable<MovieStudio> MoviesStudios { get; private set; }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesStudiosManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesStudios = await MoviesStudiosManager.GetAllAsync();
    }
}
