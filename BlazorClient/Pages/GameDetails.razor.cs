using Domain;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorClient.Pages;

public partial class GameDetails
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

    public void SetRatingPreview(byte ratingToSet)
    {
        var a = ratingToSet;
    }

    public void RatePost(byte ratingToSet)
    {
        var a = ratingToSet;
    }
}
