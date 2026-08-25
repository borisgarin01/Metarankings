using BlazorClient.Modal;

namespace BlazorClient.Components.PagesComponents.Common;

public partial class ModalDialog : ComponentBase
{
    private bool IsVisible = false;
    private ModalDialogParameters Parameters { get; set; } = new();

    [Inject] 
    private IModalService ModalService { get; set; } = default!;

    protected override void OnInitialized()
    {
        ModalService.OnShow += OnShowHandler;
        ModalService.OnClose += OnCloseHandler;
    }

    private async Task OnShowHandler(ModalDialogParameters parameters)
    {
        Parameters = parameters;
        IsVisible = true;
        StateHasChanged();
        await Task.CompletedTask;
    }

    private async Task OnCloseHandler()
    {
        IsVisible = false;
        StateHasChanged();
        await Task.CompletedTask;
    }

    private async Task ModalOk()
    {
        IsVisible = false;
        if (Parameters.OnOk is not null)
        {
            await Parameters.OnOk.Invoke();
        }
        StateHasChanged();
    }

    private async Task ModalCancel()
    {
        IsVisible = false;
        if (Parameters.OnCancel is not null)
        {
            await Parameters.OnCancel.Invoke();
        }
        StateHasChanged();
    }

    private async Task CloseOnOverlay(MouseEventArgs e)
    {
        IsVisible = false;
        if (Parameters.OnCancel is not null)
        {
            await Parameters.OnCancel.Invoke();
        }
        StateHasChanged();
    }
}