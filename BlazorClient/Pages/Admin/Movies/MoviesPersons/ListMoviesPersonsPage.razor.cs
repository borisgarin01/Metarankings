using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesPersons;

public partial class ListMoviesPersonsPage : CancellableComponentBase
{
    public IEnumerable<MoviePerson>? Entities { get; private set; }

    public string Filter { get; set; } = string.Empty;

    public IEnumerable<MoviePerson> FilteredEntities => string.IsNullOrWhiteSpace(Filter)
        ? Entities ?? Enumerable.Empty<MoviePerson>()
        : (Entities ?? Enumerable.Empty<MoviePerson>()).Where(e => e.Name.Contains(Filter.Trim(), StringComparison.OrdinalIgnoreCase));

    [Inject]
    public IWebManager<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Entities = await WebManager.GetAllAsync(DisposalToken);
    }
}
