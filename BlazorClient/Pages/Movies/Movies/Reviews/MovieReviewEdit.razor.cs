using Blazored.Toast.Services;
using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;

namespace BlazorClient.Pages.Movies.Movies.Reviews;

public partial class MovieReviewEdit : ComponentBase
{
    private int hoverScore = 0;

    [Parameter]
    public long Id { get; set; }

    public long MovieId { get; set; }

    private UpdateMovieViewerReviewModel? UpdateMovieViewerReviewModel { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    private string GetStarImage(int starIndex, bool isActive)
    {
        return isActive ? "/images/rating_on.gif" : "/images/rating_off.gif";
    }

    private void SetScore(int score)
    {
        if (UpdateMovieViewerReviewModel is not null)
        {
            UpdateMovieViewerReviewModel.Score = score;
            hoverScore = 0;
            StateHasChanged();
        }
    }

    private void HoverScore(int score)
    {
        hoverScore = score;
        StateHasChanged();
    }

    private void ResetHover()
    {
        hoverScore = 0;
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("AuthorizedClient");
            var movieReview = await client.GetFromJsonAsync<MovieViewerReview>($"/api/movies/MoviesViewersReviews/{Id}");

            if (movieReview is not null)
            {
                MovieId = movieReview.MovieId;

                UpdateMovieViewerReviewModel = new UpdateMovieViewerReviewModel
                {
                    Score = movieReview.Score,
                    TextContent = movieReview.TextContent ?? string.Empty
                };
            }
            else
            {
                ToastService.ShowError("Review not found");
                NavigationManager.NavigateTo("/");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Failed to load review: {ex.Message}");
            NavigationManager.NavigateTo("/");
        }
    }

    public async Task UpdateAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("AuthorizedClient");
            var response = await client.PutAsJsonAsync($"/api/movies/MoviesViewersReviews/{Id}", UpdateMovieViewerReviewModel);

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Review updated successfully!");
                NavigationManager.NavigateTo($"/movies/details/{MovieId}", true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ToastService.ShowError($"Failed to update: {error}");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
    }
}
