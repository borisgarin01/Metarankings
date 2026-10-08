using Domain.RequestsModels.Games.Genres;
using WebManagers;
using Domain.Games;

namespace BlazorClient.Pages.Admin.Games.Genres;

public partial class ListGenresPage : CancellableComponentBase
{
    public IEnumerable<Genre> Genres { get; private set; }

    [Inject]
    public IWebManager<Genre, AddGameGenreModel, UpdateGameGenreModel> WebManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Genres = await WebManager.GetAllAsync(DisposalToken);
    }
}
