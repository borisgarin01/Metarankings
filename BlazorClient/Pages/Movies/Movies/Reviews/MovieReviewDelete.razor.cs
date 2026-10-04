using Blazored.Toast.Services;
using Domain.Reviews;

namespace BlazorClient.Pages.Movies.Movies.Reviews;

public partial class MovieReviewDelete : ComponentBase
{
    [Parameter]
    public long Id { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    public MovieViewerReview MovieReview { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        MovieReview = await HttpClientFactory.CreateClient("AuthorizedClient").GetFromJsonAsync<MovieViewerReview>($"/api/movies/MoviesViewersReviews/{Id}");
    }

    public async Task DeleteAsync()
    {
        HttpResponseMessage httpResponseMessage = await HttpClientFactory.CreateClient("AuthorizedClient").DeleteAsync($"/api/movies/MoviesViewersReviews/{Id}");
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo($"/movies/details/{MovieReview.MovieId}", true);
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
