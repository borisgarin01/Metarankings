using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;

namespace BlazorClient.Pages.Games.Admin.Genres;

public partial class AddGenrePage : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public AddGenreModel AddGenreModel { get; } = new AddGenreModel();

    public async Task AddGenreAsync()
    {
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync("/api/Genres", AddGenreModel);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/genres/list-genres");
        else
            if (httpResponseMessage is not null)
            await JSRuntime.InvokeVoidAsync("alert", await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
