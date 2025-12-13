using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesStudios;

public partial class AddMovieStudioPage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesStudiosManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public async Task AddMovieStudioAsync()
    {
        await MoviesStudiosManager.AddAsync(new AddMovieStudioModel(Name));

        NavigationManager.NavigateTo("/movies/movies-studios/movies-studios-list");
    }
}
