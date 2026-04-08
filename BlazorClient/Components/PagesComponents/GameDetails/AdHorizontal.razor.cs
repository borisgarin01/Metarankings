namespace BlazorClient.Components.PagesComponents.GameDetails;

public partial class AdHorizontal : ComponentBase
{
    [Parameter, EditorRequired]
    public string AdId { get; set; } = "R-A-201169-7";

    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await JSRuntime.InvokeVoidAsync("renderYandexAd", AdId, $"yandex_rtb_{AdId}-1");
    }
}
