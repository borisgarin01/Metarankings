using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies
{
    public partial class RemoveMovieGenrePage : ComponentBase
    {
        [Parameter]
        public long Id { get; set; }

        public MovieGenre MovieGenre { get; private set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel> MoviesGenresWebManager { get; private set; }

        [Inject]
        public IJSRuntime JSRuntime { get; private set; }

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
                await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
