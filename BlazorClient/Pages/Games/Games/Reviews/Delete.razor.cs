using Domain.Reviews;

namespace BlazorClient.Pages.Games.Games.Reviews
{
    public partial class Delete : ComponentBase
    {
        [Parameter]
        public long Id { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public GameReview GameReview { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            GameReview = await HttpClient.GetFromJsonAsync<GameReview>(@$"/api/Games/GamesGamersReviews/{Id}");
        }

        public async Task DeleteAsync()
        {
            HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"/api/Games/GamesGamersReviews/{Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
                NavigationManager.NavigateTo($"/games/Details/{GameReview.GameId}", true);
            else
                await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
