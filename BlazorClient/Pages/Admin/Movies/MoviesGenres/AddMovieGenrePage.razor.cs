using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres;

public partial class AddMovieGenrePage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public async Task AddMovieGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesGenresManager.AddAsync(new AddMovieGenreModel(Name));
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/movies/movies-genres/list-genres");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
