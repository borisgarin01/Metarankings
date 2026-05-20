using Blazored.Toast.Services;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesStudios;

public partial class RemoveMovieStudioPage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public MovieStudio MovieStudio { get; private set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel> MoviesStudiosWebManager { get; private set; }

    [Inject]
    public IToastService ToastService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        MovieStudio = await MoviesStudiosWebManager.GetAsync(Id);
    }

    public async Task RemoveMovieStudioAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesStudiosWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/movies/movies-studios/movies-studios-list");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
