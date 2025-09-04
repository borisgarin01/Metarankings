using Domain.Games;
using Domain.Movies;

namespace BlazorClient.Pages.Admin.Movies;

public sealed partial class AddMoviePage : ComponentBase
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [EditorRequired]
    public string OriginalName { get; set; }

    [EditorRequired]
    public string Name { get; set; }

    public IEnumerable<Genre> GenresToSelectFrom { get; set; }

    public IEnumerable<MovieStudio> StudiosToSelectFrom { get; set; }

    public List<Genre> SelectedGenres { get; set; } = new List<Genre>();
    public List<MovieStudio> SelectedMoviesStudios { get; set; } = new List<MovieStudio>();

    [EditorRequired]
    public DateTime? PremierDate { get; set; }

    [EditorRequired]
    public string Description { get; set; }
    public List<MovieDirector> MovieDirectors { get; set; }

    public async Task AddMovie()
    {
        await JSRuntime.InvokeVoidAsync("alert", "Submit");
    }
}
