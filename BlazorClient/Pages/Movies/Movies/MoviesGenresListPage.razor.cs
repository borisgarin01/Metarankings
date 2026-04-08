using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using WebManagers;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MoviesGenresListPage : ComponentBase
{
    [Inject]
    public IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresManager { get; set; }

    public IEnumerable<MovieGenre> MoviesGenres { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesGenres = await MoviesGenresManager.GetAllAsync();
    }
}
