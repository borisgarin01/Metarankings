using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesDirectors;

public partial class AddMovieDirectorPage : ComponentBase
{
    public string Name { get; set; }

    [Inject]
    public IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel> MoviesDirectorsManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; private set; }
    public IJSRuntime JSRuntime { get; private set; }

    public async Task AddMovieDirectorAsync()
    {
        HttpResponseMessage httpResponseMessage = await MoviesDirectorsManager.AddAsync(new AddMovieDirectorModel(Name));

        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/movies/movies-directors/movies-directors-list");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
