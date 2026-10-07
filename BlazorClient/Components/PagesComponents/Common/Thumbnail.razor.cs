using Domain.Common;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorClient.Components.PagesComponents.Common;

public sealed partial class Thumbnail : ComponentBase
{
    [Parameter, EditorRequired]//Games/Movies
    [AllowedValues("Games", "Movies")]
    public string Prefix { get; set; }

    [Parameter, EditorRequired]
    public int ReleaseYear { get; set; }

    [Parameter, EditorRequired]
    public int ReleaseMonth { get; set; }

    [Parameter, EditorRequired]
    public string ImageSource { get; set; }

    [Parameter, EditorRequired]
    public string Name { get; set; }

    [Parameter]
    public string? Trailer { get; set; }

    private bool IsTrailerOpen { get; set; }

    private bool HasTrailer => !string.IsNullOrWhiteSpace(Trailer);

    private string AutoplayTrailerUrl
    {
        get
        {
            string embedUrl = TrailerUrl.ToEmbed(Trailer)!;
            return embedUrl.Contains('?') ? $"{embedUrl}&autoplay=1" : $"{embedUrl}?autoplay=1";
        }
    }

    private void OpenTrailer() => IsTrailerOpen = true;

    private void CloseTrailer() => IsTrailerOpen = false;

    private void OnTrailerKeyDown(KeyboardEventArgs e)
    {
        if (e.Key is "Enter" or " ")
        {
            OpenTrailer();
        }
    }
}
