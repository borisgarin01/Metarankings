namespace BlazorClient.Components.PagesComponents.Common;

public partial class ToTopButton : ComponentBase
{

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initToTop");
        }
    }

    private async Task ScrollToTop()
    {
        await JS.InvokeVoidAsync("scrollToTop");
    }
}
