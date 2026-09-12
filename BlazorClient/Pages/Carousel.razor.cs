using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorClient.Pages;

public partial class Carousel : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Parameter] public int Interval { get; set; } = 7000;
    [Parameter] public bool Autoplay { get; set; } = true;
    [Parameter] public bool ShowControls { get; set; } = true;
    [Parameter] public bool ShowIndicators { get; set; } = true;
    [Parameter] public string AdditionalClass { get; set; } = "";
    [Parameter] public EventCallback<int> OnSlideChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Список дочерних компонентов CarouselItem (компонентов, не моделей!)
    private readonly List<CarouselItem> _items = new();

    private readonly string carouselId = "carousel-" + Guid.NewGuid().ToString("N")[..8];
    private DotNetObjectReference<Carousel>? _dotNetRef;
    private bool _started;
    private int _lastItemsCount = -1;

    internal void AddItem(CarouselItem item)
    {
        if (_items.Contains(item)) return;

        item.Index = _items.Count;
        item.IsActive = _items.Count == 0;
        _items.Add(item);
        StateHasChanged();
    }

    internal void RemoveItem(CarouselItem item)
    {
        if (!_items.Remove(item)) return;

        for (int i = 0; i < _items.Count; i++)
            _items[i].Index = i;

        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!Autoplay) return;

        var count = _items.Count;

        if (firstRender || count != _lastItemsCount)
        {
            _lastItemsCount = count;
            if (count == 0) return;

            _dotNetRef ??= DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("startCarousel", carouselId, Interval, _dotNetRef);
            _started = true;
        }
    }

    [JSInvokable]
    public async Task OnSlideChangedFromJs(int index)
    {
        await OnSlideChanged.InvokeAsync(index);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_started) return;

        try { await JS.InvokeVoidAsync("stopCarousel", carouselId); }
        catch { /* компонент уже удалён */ }

        _dotNetRef?.Dispose();
        _dotNetRef = null;
        _started = false;
    }
}