using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Genres;

public partial class AddGenrePage : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IWebManager<Genre, AddGenreModel, UpdateGenreModel> GenresWebManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddGenreModel AddGenreModel { get; } = new AddGenreModel();

    public async Task AddGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await GenresWebManager.AddAsync(AddGenreModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/genres/list-genres");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
