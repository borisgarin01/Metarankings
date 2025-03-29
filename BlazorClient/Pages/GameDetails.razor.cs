using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages;

public partial class GameDetails : ComponentBase
{
    public Game Game { get; set; }

    [Parameter]
    public int Id { get; set; }

    [Inject]
    public HttpClient HttpClient { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Game = await HttpClient.GetFromJsonAsync<Game>($"/api/Games/{Id}");
    }

    private byte currentHoverRating = 0;
    private byte selectedRating = 0; // Optional: to store the actual selected rating

    private void SetRatingPreview(byte rating)
    {
        currentHoverRating = rating;
    }

    private void RatePost(byte rating)
    {
        if (selectedRating == 0)
            selectedRating = rating;
        currentHoverRating = rating;
        // Add your post rating logic here
    }
}
