using Domain.Movies;

namespace BlazorClient.Components.PagesComponents.Home;

public partial class MoviesDropdownComponent : ComponentBase
{
    [Parameter] public IEnumerable<MovieGenre> Genres { get; set; }

    private List<int> GetYears()
    {
        var currentYear = DateTime.Now.Year;
        return Enumerable.Range(currentYear - 8, 9).Reverse().ToList();
    }
}
