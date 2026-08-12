namespace BlazorClient.Modal;

public class ModalDialogParameters
{
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public ModalDialogType DialogType { get; set; } = ModalDialogType.Ok;
    public Func<Task>? OnOk { get; set; }
    public Func<Task>? OnCancel { get; set; }
}