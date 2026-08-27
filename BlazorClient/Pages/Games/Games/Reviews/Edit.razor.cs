using Blazored.Toast.Services;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.Reviews;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Games.Games.Reviews;

public partial class Edit : ComponentBase
{
    private int hoverScore = 0;

    [Parameter]
    public long Id { get; set; }

    public long GameId { get; set; }

    private UpdateGamePlayerReviewModel? UpdateGamePlayerReviewModel { get; set; }

    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    private string GetStarImage(int starIndex, bool isActive)
    {
        // Если звезда активна - rating_on
        if (isActive)
        {
            return "/images/rating_on.gif";
        }

        // Если звезда НЕ активна - rating_off
        return "/images/rating_off.gif";
    }

    private void SetScore(int score)
    {
        if (UpdateGamePlayerReviewModel is not null)
        {
            UpdateGamePlayerReviewModel.Score = score;
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
            var gameReview = await client.GetFromJsonAsync<GameReview>($"/api/Games/GamesGamersReviews/{Id}");

            if (gameReview is not null)
            {
                GameId = gameReview.GameId;

                UpdateGamePlayerReviewModel = new UpdateGamePlayerReviewModel
                {
                    Score = gameReview.Score,
                    TextContent = gameReview.TextContent ?? string.Empty
                };
            }
            else
            {
                ToastService.ShowError("Review not found");
                NavigationManager.NavigateTo("/games");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Failed to load review: {ex.Message}");
            NavigationManager.NavigateTo("/games");
        }
    }

    public async Task UpdateAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("AuthorizedClient");
            var response = await client.PutAsJsonAsync($"/api/Games/gamesGamersReviews/{Id}", UpdateGamePlayerReviewModel);

            if (response.IsSuccessStatusCode)
            {
                ToastService.ShowSuccess("Review updated successfully!");
                NavigationManager.NavigateTo($"/games/Details/{GameId}", true);
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