using Domain.Games;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Games.Games;

public partial class GameDetails : ComponentBase
{
    public GameModel Game { get; set; }

    [Parameter]
    public long Id { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    private byte currentHoverRating = 0;
    private byte selectedRating = 0;
    private bool hasRated = false;

    protected override async Task OnParametersSetAsync()
    {
        Game = await HttpClient.GetFromJsonAsync<GameModel>($"/api/Games/{Id}");
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