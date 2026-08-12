namespace BlazorClient.Modal;

public interface IModalService
{
    event Func<ModalDialogParameters, Task>? OnShow;
    event Func<Task>? OnClose;

    Task ShowAsync(string title, string text, ModalDialogType dialogType = ModalDialogType.Ok);
    Task ShowAsync(ModalDialogParameters parameters);
    Task CloseAsync();
}
