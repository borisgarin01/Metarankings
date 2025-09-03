
using Domain.Movies;

namespace BlazorClient.Pages.Movies.Movies;

public partial class MovieDetails : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    public Movie Movie { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Movie = await HttpClient.GetFromJsonAsync<Movie>($"/api/Movies/{Id}");
    }
}
