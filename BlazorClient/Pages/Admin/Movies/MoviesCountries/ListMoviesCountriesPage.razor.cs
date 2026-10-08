using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesCountries;

public partial class ListMoviesCountriesPage : ComponentBase
{
    public IEnumerable<MovieCountry>? Entities { get; private set; }

    public string Filter { get; set; } = string.Empty;

    public IEnumerable<MovieCountry> FilteredEntities => string.IsNullOrWhiteSpace(Filter)
        ? Entities ?? Enumerable.Empty<MovieCountry>()
        : (Entities ?? Enumerable.Empty<MovieCountry>()).Where(e => e.Name.Contains(Filter.Trim(), StringComparison.OrdinalIgnoreCase));

    [Inject]
    public IWebManager<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Entities = await WebManager.GetAllAsync();
    }
}
