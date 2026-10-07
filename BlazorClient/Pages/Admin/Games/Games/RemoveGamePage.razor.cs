using Domain.Games;
using Domain.RequestsModels.Games;

namespace BlazorClient.Pages.Admin.Games.Games;

public partial class RemoveGamePage : RemoveEntityPageBase<Game, AddGameModel, UpdateGameModel>
{
    protected override string ListUrl => "/admin/games/games/list-games";
}
