using Domain.Games;
using Domain.RequestsModels.Games.Genres;

namespace BlazorClient.Pages.Admin.Games.Genres;

public partial class RemoveGenrePage : RemoveEntityPageBase<Genre, AddGameGenreModel, UpdateGameGenreModel>
{
    protected override string ListUrl => "/admin/games/genres/list-genres";
}
