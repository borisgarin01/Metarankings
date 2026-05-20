using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Genres;

public partial class AddGenrePage : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GenresWebManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    public AddGameGenreModel AddGenreModel { get; } = new AddGameGenreModel();

    public async Task AddGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await GenresWebManager.AddAsync(AddGenreModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/games/genres/list-genres");
        else
            if (httpResponseMessage is not null)
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
