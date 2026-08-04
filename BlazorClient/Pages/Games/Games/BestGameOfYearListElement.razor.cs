using Domain.Games;

namespace BlazorClient.Pages.Games.Games
{
    public partial class BestGameOfYearListElement : ComponentBase
    {
        [Parameter, EditorRequired]
        public Game Game { get; set; }

        [Parameter, EditorRequired]
        public int Order { get; set; }
    }
}
