using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Genres;

public partial class RemoveGenrePage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Genre Genre { get; private set; }

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> GenresWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genre = await GenresWebManager.GetAsync(Id);
    }

    public async Task RemoveGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await GenresWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/games/genres/list-genres");
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}