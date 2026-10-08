using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;

namespace BlazorClient.Pages.Admin.Movies.MoviesCountries;

public partial class RemoveMovieCountryPage : RemoveEntityPageBase<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel>
{
    protected override string ListUrl => "/admin/movies/movies-countries/movies-countries-list";
}
