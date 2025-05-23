﻿using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages;

public partial class GameDetails : ComponentBase
{
    public Domain.GameModel Game { get; set; }

    [Parameter]
    public long Id { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    private byte currentHoverRating = 0;
    private byte selectedRating = 0;
    private bool hasRated = false;

    protected override async Task OnParametersSetAsync()
    {
        Game = await HttpClient.GetFromJsonAsync<Domain.GameModel>($"/api/Games/{Id}");
        SetRatingPreview(Convert.ToByte(Game.Score.HasValue ? Game.Score.Value : 0));
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
            Game.ScoresCount++;
            hasRated = true;
            // Consider adding API call to persist the rating
            StateHasChanged();
        }
    }
}