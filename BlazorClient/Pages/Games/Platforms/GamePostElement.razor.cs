using Domain.Games;

namespace BlazorClient.Pages.Games.Platforms
{
    public partial class GamePostElement : ComponentBase
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Parameter, EditorRequired]
        public Game Game { get; set; }
    }
}
