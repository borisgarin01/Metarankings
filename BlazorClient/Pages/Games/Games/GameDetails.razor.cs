using Domain.Games;

namespace BlazorClient.Pages.Games.Games;

public partial class GameDetails : ComponentBase
{
    public Game Game { get; set; }

    [Parameter]
    public long Id { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    private byte currentHoverRating = 0;
    private byte selectedRating = 0;
    private bool hasRated = false;

    protected override async Task OnParametersSetAsync()
    {
        Game = await HttpClient.GetFromJsonAsync<Game>($"/api/Games/{Id}");
        SetRatingPreview(0);
    }

    private void SetRatingPreview(byte rating)
    {
        currentHoverRating = rating;
        StateHasChanged();
    }

    private void RatePost(byte rating)
    {
        if (!hasRated)
        {
            selectedRating = rating;
            hasRated = true;
            // Consider adding API call to persist the rating
            StateHasChanged();
        }
    }
}