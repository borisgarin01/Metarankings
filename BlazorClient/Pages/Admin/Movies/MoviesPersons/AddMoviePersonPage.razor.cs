using System.ComponentModel.DataAnnotations;
using Blazored.Toast.Services;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesPersons;
using WebManagers;

namespace BlazorClient.Pages.Admin.Movies.MoviesPersons;

public partial class AddMoviePersonPage : CancellableComponentBase
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Inject]
    public IWebManager<MoviePerson, AddMoviePersonModel, UpdateMoviePersonModel> WebManager { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    public async Task AddAsync()
    {
        HttpResponseMessage httpResponseMessage = await WebManager.AddAsync(new AddMoviePersonModel(Name.Trim()), DisposalToken);

        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/movies/movies-persons/movies-persons-list");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync(DisposalToken));
    }
}
