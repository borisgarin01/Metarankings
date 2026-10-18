using Domain.Common;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorClient.Components.PagesComponents.Common;

public sealed partial class TrailerButton : ComponentBase
{
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
