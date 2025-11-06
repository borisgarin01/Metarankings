using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies;

public partial class AddMovieDirectorPage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MovieDirectorManager { get; set; }

    public async Task AddMovieDirectorAsync()
    {
        await MovieDirectorManager.AddAsync(new AddMovieDirectorModel(Name));
    }
}
