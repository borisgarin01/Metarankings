namespace BlazorClient.Components.PagesComponents.Common;

public partial class SingleSelect<TItem, TId> : ComponentBase
{
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public TId? SelectedId { get; set; }
    [Parameter] public EventCallback<TId?> SelectedIdChanged { get; set; }
    [Parameter] public Func<TItem, TId?> IdSelector { get; set; } // Changed to nullable
    [Parameter] public Func<TItem, string> DisplaySelector { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Select an item...";

    private bool IsOpen { get; set; }
    private string SearchTerm { get; set; } = string.Empty;

    private TItem? SelectedItem => Items.FirstOrDefault(x => SelectedId != null && SelectedId.Equals(IdSelector(x)));
    private List<TItem> FilteredItems => string.IsNullOrWhiteSpace(SearchTerm)
        ? Items
        : Items.Where(x => DisplaySelector(x).Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

    private TId? GetId(TItem item) => IdSelector(item); // Returns nullable
    private string GetDisplayText(TItem item) => DisplaySelector(item);

    private void ToggleDropdown()
    {
        IsOpen = !IsOpen;
        if (!IsOpen)
        {
            SearchTerm = string.Empty;
        }
    }

    private async Task SelectItem(TItem item)
    {
        TId? id = GetId(item);
        if (SelectedId != null && SelectedId.Equals(id))
        {
            // Если кликнули на уже выбранный элемент - снимаем выбор
            SelectedId = default;
        }
        else
        {
            SelectedId = id;
        }
        await SelectedIdChanged.InvokeAsync(SelectedId);
        IsOpen = false;
        StateHasChanged();
    }
}
