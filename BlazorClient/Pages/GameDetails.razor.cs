using BlazorClient.Components.PagesComponents;
using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages;

public partial class GameDetails
{
    public Domain.Game Game { get; set; }

    [Parameter]
    public int Id { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    private byte currentHoverRating = 0;
    private byte selectedRating = 0;
    private bool hasRated = false;

    protected override async Task OnParametersSetAsync()
    {
        Game = await HttpClient.GetFromJsonAsync<Domain.Game>($"/api/Games/{Id}");
        SetRatingPreview(Convert.ToByte(Game.Score.Value));
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