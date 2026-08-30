using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesStudios;

public partial class ListMoviesStudiosPage : ComponentBase
{
    private IEnumerable<MovieStudio> moviesStudios;

    public IEnumerable<MovieStudio> MoviesStudios
    {
        get => moviesStudios;
        set
        {
            moviesStudios = value;
            StateHasChanged();
        }
    }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesStudiosWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesStudios = await MoviesStudiosWebManager.GetAllAsync();
    }
}
