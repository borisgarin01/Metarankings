using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesStudios;

public partial class AddMovieStudioPage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesGenresManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }
}
