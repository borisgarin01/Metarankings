using Domain.RequestsModels.Movies.MoviesGenres;
using WebManagers;
using Domain.Movies;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres;

public partial class ListMoviesGenresPage : CancellableComponentBase
{
    public IEnumerable<Genre> Genres { get; private set; }

    [Inject]
    public IWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Genres = await WebManager.GetAllAsync(DisposalToken);
    }
}
