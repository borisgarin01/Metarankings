namespace BlazorClient.Components.PagesComponents.Common;

public partial class MultiSelect<TItem, TId> : ComponentBase
{
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public List<TId> SelectedIds { get; set; } = new();
    [Parameter] public EventCallback<List<TId>> SelectedIdsChanged { get; set; }
    [Parameter] public Func<TItem, TId> IdSelector { get; set; }
    [Parameter] public Func<TItem, string> DisplaySelector { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Select items...";
    [Parameter] public string LabelPlural { get; set; } = "items";

    private bool IsOpen { get; set; }
    private string SearchTerm { get; set; } = string.Empty;

    private List<TItem> SelectedItems => Items.Where(x => SelectedIds.Contains(IdSelector(x))).ToList();
    private List<TItem> FilteredItems => string.IsNullOrWhiteSpace(SearchTerm)
        ? Items
        : Items.Where(x => DisplaySelector(x).Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

    private TId GetId(TItem item) => IdSelector(item);
    private string GetDisplayText(TItem item) => DisplaySelector(item);

    private void ToggleDropdown()
    {
        IsOpen = !IsOpen;
        if (!IsOpen)
        {
            SearchTerm = string.Empty;
        }
    }

    private async Task ToggleSelection(TItem item)
    {
        TId? id = GetId(item);
        if (SelectedIds.Contains(id))
        {
            SelectedIds.Remove(id);
        }
        else
        {
            SelectedIds.Add(id);
        }
        await SelectedIdsChanged.InvokeAsync(SelectedIds);
        StateHasChanged();
    }

    private async Task RemoveItem(TId id)
    {
        if (SelectedIds.Contains(id))
        {
            SelectedIds.Remove(id);
            await SelectedIdsChanged.InvokeAsync(SelectedIds);
            StateHasChanged();
        }
    }
}
