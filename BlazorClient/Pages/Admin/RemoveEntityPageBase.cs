using Blazored.Toast.Services;
using WebManagers;

namespace BlazorClient.Pages.Admin;

/// <summary>
/// Страница подтверждения удаления сущности: загружает её по Id, удаляет и возвращает к списку.
/// </summary>
public abstract class RemoveEntityPageBase<T, TAdd, TUpdate> : ComponentBase where T : class
{
    [Parameter]
    public long Id { get; set; }

    [Inject]
    public IWebManager<T, TAdd, TUpdate> WebManager { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    public T? Entity { get; private set; }

    /// <summary>
    /// Адрес списка, куда возвращаемся после удаления.
    /// </summary>
    protected abstract string ListUrl { get; }

    protected override async Task OnInitializedAsync()
    {
        Entity = await WebManager.GetAsync(Id);
    }

    public async Task RemoveAsync()
    {
        HttpResponseMessage httpResponseMessage = await WebManager.DeleteAsync(Id);
        if (httpResponseMessage.IsSuccessStatusCode)
            NavigationManager.NavigateTo(ListUrl);
        else
            ToastService.ShowError(await httpResponseMessage.Content.ReadAsStringAsync());
    }
}
