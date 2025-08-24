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

    protected override async Task OnInitializedAsync()
    {
        Genre = await GenresWebManager.GetAsync(Id);
    }
}