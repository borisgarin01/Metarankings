using Domain.Games;
using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MovieGenreMoviesListPage : ComponentBase
{
    [Parameter]
    public long GenreId { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    public Domain.Movies.Genre Genre { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Genre = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<Domain.Movies.Genre>($"/api/Movies/Genres/{GenreId}");
    }
}
