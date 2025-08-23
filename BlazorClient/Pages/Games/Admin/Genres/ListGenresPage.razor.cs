using Domain.Games;

namespace BlazorClient.Pages.Games.Admin.Genres
{
    public partial class ListGenresPage : ComponentBase
    {
        public IEnumerable<Genre> Genres { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Genres = await HttpClient.GetFromJsonAsync<IEnumerable<Genre>>(@"/api/Genres");
        }
    }
}
