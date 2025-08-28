using Domain.Games;
using Domain.RequestsModels.Games.Genres;
using WebManagers;

namespace BlazorClient.Pages.Games.Admin.Genres;

public partial class RemoveGenrePage : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    public Genre Genre { get; private set; }

    [Inject]
    public IWebManager<Genre, AddGenreModel, UpdateGenreModel> GenresWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Genre = await GenresWebManager.GetAsync(Id);
    }

    public async Task RemoveGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await GenresWebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            NavigationManager.NavigateTo("/admin/genres/list-genres");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}