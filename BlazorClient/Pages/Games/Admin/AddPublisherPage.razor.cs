using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Publishers;
using System.Net.Http.Json;

namespace BlazorClient.Pages.Games.Admin;

public partial class AddPublisherPage : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddPublisherModel AddPublisherModel { get; } = new AddPublisherModel();

    public async Task AddPublisherAsync()
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync<AddPublisherModel>("/api/Publishers", AddPublisherModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/list-publishers");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }

}
