namespace BlazorClient.Pages;

public partial class CarouselProgress : ComponentBase
{
    [Parameter] public double Width { get; set; } = 0;
    [Parameter] public int Duration { get; set; } = 5000;
    [Parameter] public bool IsPaused { get; set; } = false;
    [Parameter] public string Height { get; set; } = "4px";
    [Parameter] public string TrackColor { get; set; } = "rgba(255,255,255,0.3)";
    [Parameter] public string BarColor { get; set; } = "#ffffff";
    [Parameter] public string BorderRadius { get; set; } = "2px";
    [Parameter] public string Position { get; set; } = "bottom";

    private string GetPositionStyle()
    {
        return Position switch
        {
            "top" => "top: 0;",
            "bottom" => "bottom: 0;",
            "overlay" => "position: absolute; bottom: 0;",
            _ => "bottom: 0;"
        };
    }
}
