namespace BlazorClient.Components.PagesComponents.Common;

public partial class RatingComponent : ComponentBase
{
    [Parameter] public double? Score { get; set; }
    [Parameter] public long ScoresCount { get; set; }
    [Parameter] public byte SelectedRating { get; set; }
    [Parameter] public byte CurrentHoverRating { get; set; }
    [Parameter] public EventCallback<byte> OnRate { get; set; }
    [Parameter] public EventCallback<byte> OnHover { get; set; }

    private string GetRatingImage(int rating)
    {
        if (SelectedRating == 0)
        {
            return rating <= CurrentHoverRating ? "rating_on.gif" : "rating_off.gif";
        }
        return rating <= SelectedRating ? "rating_on.gif" : "rating_off.gif";
    }
}
