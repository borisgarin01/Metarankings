using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesDirectors;

public partial class RemoveMovieDirectorPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public MovieDirector MovieDirector { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MoviesDirectorsWebManager { get; private set; }

    [Inject]
    public IJSRuntime JSRuntime { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        MovieDirector = await MoviesDirectorsWebManager.GetAsync(Id);
    }

    public async Task RemoveMovieDirectorAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesDirectorsWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/movies/movies-directors/movies-directors-list");
        else
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
