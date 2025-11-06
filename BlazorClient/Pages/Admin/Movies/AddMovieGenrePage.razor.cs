using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies;

public partial class AddMovieGenrePage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresManager { get; set; }

    public async Task AddMovieGenreAsync()
    {
        await MoviesGenresManager.AddAsync(new AddMovieGenreModel(Name));
    }
}
