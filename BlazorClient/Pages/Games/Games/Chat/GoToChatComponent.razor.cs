using Domain.Games;

namespace BlazorClient.Pages.Games.Games.Chat;

public partial class GoToChatComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public long GameId { get; set; }
}
