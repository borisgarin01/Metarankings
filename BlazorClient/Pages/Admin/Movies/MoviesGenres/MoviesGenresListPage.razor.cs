using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres
{
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
}
