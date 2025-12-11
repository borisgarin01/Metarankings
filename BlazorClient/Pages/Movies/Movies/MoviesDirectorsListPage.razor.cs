using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MoviesDirectorsListPage : ComponentBase
{
    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MoviesDirectorsManager { get; set; }

    public IEnumerable<MovieDirector> MoviesDirectors { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        MoviesDirectors = await MoviesDirectorsManager.GetAllAsync();
    }
}
