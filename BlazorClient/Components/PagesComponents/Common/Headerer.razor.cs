using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Movies.MoviesGenres;
using WebManagers;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class Headerer : ComponentBase
{
    [Inject]
    public IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; set; }

    [Inject]
    public IWebManager<Platform, AddPlatformModel, UpdatePlatformModel> PlatformsWebManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GamesGenresWebManager { get; set; }

    public IEnumerable<MovieGenre> MoviesGenres { get; set; }
    public IEnumerable<Platform> Platforms { get; set; }
    public IEnumerable<Genre> GamesGenres { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Task<IEnumerable<MovieGenre>> moviesGenresGettingTask = MoviesGenresWebManager.GetAllAsync();
        Task<IEnumerable<Platform>> platformsGettingTask = PlatformsWebManager.GetAllAsync();
        Task<IEnumerable<Genre>> gamesGenresGettingTask = GamesGenresWebManager.GetAllAsync();

        await Task.WhenAll(moviesGenresGettingTask, platformsGettingTask, gamesGenresGettingTask).ContinueWith(b =>
        {
            MoviesGenres = moviesGenresGettingTask.Result;
            Platforms = platformsGettingTask.Result;
            GamesGenres = gamesGenresGettingTask.Result;
        });
    }
}
