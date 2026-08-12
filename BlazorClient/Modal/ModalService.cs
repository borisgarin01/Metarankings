namespace BlazorClient.Modal;

public class ModalService : IModalService
{
    public event Func<ModalDialogParameters, Task>? OnShow;
    public event Func<Task>? OnClose;

    public async Task ShowAsync(string title, string text, ModalDialogType dialogType = ModalDialogType.Ok)
    {
        if (OnShow is not null)
        {
            await OnShow.Invoke(new ModalDialogParameters
            {
                Title = title,
                Text = text,
                DialogType = dialogType
            });
        }
    }

    public async Task ShowAsync(ModalDialogParameters parameters)
    {
        if (OnShow is not null)
        {
            await OnShow.Invoke(parameters);
        }
    }

    public async Task CloseAsync()
    {
        if (OnClose is not null)
        {
            await OnClose.Invoke();
        }
    }
}
