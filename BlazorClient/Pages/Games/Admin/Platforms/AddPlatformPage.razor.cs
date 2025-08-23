using Domain.RequestsModels.Games.Platforms;

namespace BlazorClient.Pages.Games.Admin.Platforms;

public partial class AddPlatformPage : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddPlatformModel AddPlatformModel { get; } = new AddPlatformModel();
    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    public async Task AddPlatformAsync()
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Platforms", AddPlatformModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/platforms/list-platforms");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
