using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class MoviesDropdownComponent : ComponentBase
{
    // State tracks if dropdowns are active
    private bool isMoviesOpen;

    // Computed CSS classes based on C# state
    private string moviesDropdownClass => isMoviesOpen ? "show" : "";

    // Handlers to change state instantly inside WebAssembly
    private void ShowMovies() => isMoviesOpen = true;
    private void HideMovies() => isMoviesOpen = false;

    [Parameter, EditorRequired]
    public IEnumerable<Genre> Genres { get; set; }
}
