using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Movies.Movies;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.Movies;


public partial class RemoveMoviePage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Movie Movie { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public IWebManager<Movie, AddMovieModel, UpdateMovieModel> MoviesWebManager { get; set; }

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
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
