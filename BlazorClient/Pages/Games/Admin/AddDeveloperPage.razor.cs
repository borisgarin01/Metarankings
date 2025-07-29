using Domain.RequestsModels.Games.Developers;

namespace BlazorClient.Pages.Games.Admin;

public partial class AddDeveloperPage : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public AddDeveloperModel AddDeveloperModel { get; } = new AddDeveloperModel();
    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    public async Task AddDeveloperAsync()
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddDeveloperModel>("/api/Developers", AddDeveloperModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/list-developers");

    }
}
