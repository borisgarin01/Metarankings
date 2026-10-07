using Domain.Common;

namespace BlazorClient.Components.PagesComponents.Common;

public sealed partial class TrailerField : ComponentBase
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    private string? PreviewUrl => TrailerUrl.ToEmbed(Value);

    private async Task OnInputAsync(ChangeEventArgs e)
    {
        Value = e.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }
}
