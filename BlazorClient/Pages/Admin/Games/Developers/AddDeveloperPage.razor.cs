using Blazored.Toast.Services;
using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Microsoft.AspNetCore.Authorization;
using WebManagers;

namespace BlazorClient.Pages.Admin.Games.Developers;

[Authorize(Policy = "Admin")]
public partial class AddDeveloperPage : CancellableComponentBase
{
    [Inject]
    public IWebManager<Developer, AddDeveloperModel, UpdateDeveloperModel> DevelopersWebManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    public AddDeveloperModel AddDeveloperModel { get; } = new AddDeveloperModel();

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    public async Task AddDeveloperAsync()
    {
        HttpResponseMessage httpResponseMessage = await DevelopersWebManager.AddAsync(AddDeveloperModel, DisposalToken);
        if (httpResponseMessage is not null && httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo("/admin/games/developers/list-developers");
        else
            if (httpResponseMessage is not null)
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync(DisposalToken));
    }
}
