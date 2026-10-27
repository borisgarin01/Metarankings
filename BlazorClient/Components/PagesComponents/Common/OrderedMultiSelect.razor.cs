namespace BlazorClient.Components.PagesComponents.Common;

public partial class OrderedMultiSelect<TItem, TId> : ComponentBase, IAsyncDisposable
{
    [Inject]
    public IJSRuntime JS { get; set; }

    [Parameter]
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();

    [Parameter]
    public IReadOnlyList<TItem> AllItems { get; set; } = Array.Empty<TItem>();

    [Parameter]
    public Func<TItem, TId> IdSelector { get; set; } = default!;

    [Parameter]
    public Func<TItem, string> DisplaySelector { get; set; } = default!;

    [Parameter]
    public string Placeholder { get; set; } = "Выберите...";

    [Parameter]
    public string LabelPlural { get; set; } = "элементы";

    [Parameter]
    public EventCallback<IReadOnlyList<TItem>> ItemsChanged { get; set; }

    private ElementReference _listElement;
    private string? _idToAdd;
    private DotNetObjectReference<OrderedMultiSelect<TItem, TId>>? _selfRef;
    private bool _sortableInitialized;

    private IEnumerable<TItem> AvailableItems =>
        AllItems.Where(a => !Items.Any(i => EqualityComparer<TId>.Default
            .Equals(IdSelector(i), IdSelector(a))));

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // <ul> присутствует в DOM всегда, поэтому SortableJS достаточно инициализировать один раз —
        // новые элементы он подхватывает сам.
        if (firstRender)
        {
            _selfRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("sortableInterop.create", _listElement, _selfRef, nameof(OnOrderChanged));
            _sortableInitialized = true;
        }
    }

    [JSInvokable]
    public async Task OnOrderChanged(string[] orderedIds)
    {
        List<TItem> reordered = orderedIds
            .Select(id => Items.FirstOrDefault(i => IdSelector(i)?.ToString() == id))
            .Where(i => i is not null)
            .Cast<TItem>()
            .ToList();

        if (reordered.Count != Items.Count)
            return;

        await ItemsChanged.InvokeAsync(reordered);
    }

    private async Task AddAsync()
    {
        if (string.IsNullOrEmpty(_idToAdd))
            return;

        TItem? item = AvailableItems.FirstOrDefault(i => IdSelector(i)?.ToString() == _idToAdd);
        if (item is null)
            return;

        List<TItem> list = Items.ToList();
        list.Add(item);
        _idToAdd = null;

        await ItemsChanged.InvokeAsync(list);
    }

    private async Task RemoveAsync(TItem item)
    {
        List<TItem> list = Items
            .Where(i => !EqualityComparer<TId>.Default.Equals(IdSelector(i), IdSelector(item)))
            .ToList();

        await ItemsChanged.InvokeAsync(list);
    }

    public async ValueTask DisposeAsync()
    {
        if (_sortableInitialized)
        {
            try { await JS.InvokeVoidAsync("sortableInterop.destroy", _listElement); }
            catch (JSDisconnectedException) { }
        }

        _selfRef?.Dispose();
    }
}