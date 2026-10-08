using System.ComponentModel.DataAnnotations;
using Blazored.Toast.Services;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesCountries;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesCountries;

public partial class AddMovieCountryPage : ComponentBase
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Inject]
    public IWebManager<MovieCountry, AddMovieCountryModel, UpdateMovieCountryModel> WebManager { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    public async Task AddAsync()
    {
        HttpResponseMessage httpResponseMessage = await WebManager.AddAsync(new AddMovieCountryModel(Name.Trim()));

        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/movies/movies-countries/movies-countries-list");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
