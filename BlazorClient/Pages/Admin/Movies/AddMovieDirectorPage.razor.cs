using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies;

public partial class AddMovieDirectorPage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MoviesDirectorsManager { get; set; }

    public async Task AddMovieDirectorAsync()
    {
        await MoviesDirectorsManager.AddAsync(new AddMovieDirectorModel(Name));
    }
}
