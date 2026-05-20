using Blazored.Toast.Services;
using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies.Movies;
using WebManagers;
using WebManagers.Derived.Movies;

namespace BlazorClient.Pages.Admin.Movies.Movies;


public partial class RemoveMoviePage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Movie Movie { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public MoviesWebManager MoviesWebManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Movie = await MoviesWebManager.GetAsync(Id);
    }

    public async Task RemoveMovieAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/movies/movies/list-movies");
        }
        else
        {
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
