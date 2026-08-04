using Domain.Games;

namespace BlazorClient.Pages.Games.Platforms
{
    public partial class GamePostElement : ComponentBase
    {
        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; }

        [Parameter, EditorRequired]
        public Game Game { get; set; }
    }
}
