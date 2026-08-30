using Blazored.Toast.Services;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesGenres;

public partial class RemoveMovieGenrePage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Genre MovieGenre { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; private set; }

    [Inject]
    public IToastService ToastService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        MovieGenre = await MoviesGenresWebManager.GetAsync(Id);
    }

    public async Task RemoveMovieGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesGenresWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/movies/movies-genres/movies-genres-list");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
